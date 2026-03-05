using BuildingBlocks.Application.Clock;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Persistence.EfCore.Outbox;

public class OutboxCleaner<TDbContext>(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxCleanerOptions> options,
    ILogger<OutboxCleaner<TDbContext>> logger) : BackgroundService
    where TDbContext : EfCoreDbContext
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var opts = options.Value;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanBatchAsync(opts, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Outbox cleaning error");
            }

            await Task.Delay(TimeSpan.FromSeconds(opts.PollingIntervalSeconds), stoppingToken);
        }
    }

    private async Task CleanBatchAsync(OutboxCleanerOptions opts, CancellationToken cancellationToken)
    {
        await using var scope            = scopeFactory.CreateAsyncScope();
        var             context          = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var             dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var cutoff = dateTimeProvider.UtcNow.AddDays(-opts.RetentionDays);

        var processedDeleted = await context.OutboxMessages
            .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoff)
            .Take(opts.BatchSize)
            .ExecuteDeleteAsync(cancellationToken);

        if (processedDeleted > 0)
            logger.LogInformation("Deleted {Count} processed outbox messages older than {Cutoff}", processedDeleted, cutoff);
        else
            logger.LogDebug("No processed outbox messages to clean up");

        var failedDeleted = await context.OutboxMessages
            .Where(m => m.RetryCount >= opts.MaxRetryCount && m.OccurredOn < cutoff)
            .Take(opts.BatchSize)
            .ExecuteDeleteAsync(cancellationToken);

        if (failedDeleted > 0)
            logger.LogInformation("Deleted {Count} permanently failed outbox messages older than {Cutoff}", failedDeleted, cutoff);
        else
            logger.LogDebug("No permanently failed outbox messages to clean up");
    }
}
