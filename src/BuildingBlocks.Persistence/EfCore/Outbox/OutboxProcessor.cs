using System.Collections.Concurrent;
using System.Text.Json;
using BuildingBlocks.Application.Clock;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Contracts.Messaging;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Persistence.EfCore.Outbox;

public class OutboxProcessor<TDbContext>(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxProcessorOptions> options,
    ILogger<OutboxProcessor<TDbContext>> logger) : BackgroundService
    where TDbContext : EfCoreDbContext
{
    private static readonly ConcurrentDictionary<string, Type?> TypeCache = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var opts = options.Value;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(opts, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Outbox processing error");
            }

            await Task.Delay(TimeSpan.FromSeconds(opts.PollingIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(OutboxProcessorOptions opts, CancellationToken cancellationToken)
    {
        await using var scope            = scopeFactory.CreateAsyncScope();
        var             context          = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var             bus              = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var             dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var messages = await context.OutboxMessages
            .FromSqlRaw(
                """
                SELECT * FROM outbox.outbox_messages
                WHERE "ProcessedOn" IS NULL AND "RetryCount" < {0}
                ORDER BY "OccurredOn"
                LIMIT {1}
                FOR UPDATE SKIP LOCKED
                """, opts.MaxRetryCount, opts.BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var eventType = ResolveType(message.EventType);

                if (eventType is null)
                {
                    logger.LogWarning("Could not resolve type {EventType} for outbox message {MessageId}", message.EventType, message.Id);
                    message.LastErrorOn  = dateTimeProvider.UtcNow;
                    message.ErrorMessage = $"Could not resolve type: {message.EventType}";
                    message.RetryCount   = opts.MaxRetryCount;
                    await context.SaveChangesAsync(cancellationToken);
                    continue;
                }

                var integrationEvent = JsonSerializer.Deserialize(message.Payload, eventType) as IIntegrationEvent;

                if (integrationEvent is null)
                {
                    logger.LogWarning("Could not deserialize outbox message {MessageId}", message.Id);
                    message.LastErrorOn  = dateTimeProvider.UtcNow;
                    message.ErrorMessage = "Deserialization returned null";
                    message.RetryCount   = opts.MaxRetryCount;
                    await context.SaveChangesAsync(cancellationToken);
                    continue;
                }

                await bus.PublishAsync((object)integrationEvent, cancellationToken);
                message.ProcessedOn = dateTimeProvider.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                message.RetryCount++;
                message.LastErrorOn  = dateTimeProvider.UtcNow;
                message.ErrorMessage = ex.Message;
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    private static Type? ResolveType(string typeName)
    {
        return TypeCache.GetOrAdd(typeName, static name =>
            Type.GetType(name) ?? AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(name))
                .FirstOrDefault(t => t is not null));
    }
}
