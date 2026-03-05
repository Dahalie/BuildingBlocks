using BuildingBlocks.Application.Clock;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Persistence.EfCore.Inbox;

public class InboxCleaner<TDbContext>(
    IServiceScopeFactory scopeFactory,
    IOptions<InboxCleanerOptions> options,
    ILogger<InboxCleaner<TDbContext>> logger) : BackgroundService
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
                logger.LogError(ex, "Inbox cleaning error");
            }

            await Task.Delay(TimeSpan.FromSeconds(opts.PollingIntervalSeconds), stoppingToken);
        }
    }

    private async Task CleanBatchAsync(InboxCleanerOptions opts, CancellationToken cancellationToken)
    {
        await using var scope            = scopeFactory.CreateAsyncScope();
        var             context          = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var             dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var cutoff = dateTimeProvider.UtcNow.AddDays(-opts.RetentionDays);

        var deletedCount = await context.InboxMessages
            .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoff)
            .Take(opts.BatchSize)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount > 0)
            logger.LogInformation("Deleted {Count} processed inbox messages older than {Cutoff}", deletedCount, cutoff);
        else
            logger.LogDebug("No processed inbox messages to clean up");
    }
}
