using Microsoft.Extensions.Logging;
using Quartz;

namespace BuildingBlocks.Infrastructure.Jobs;

public abstract class JobBase(ILogger logger) : IJob
{
    protected ILogger Logger { get; } = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = GetType().Name;

        try
        {
            Logger.LogInformation("Job {JobName} started.", jobName);
            await ExecuteAsync(context.CancellationToken);
            Logger.LogInformation("Job {JobName} completed.", jobName);
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("Job {JobName} was cancelled.", jobName);
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Job {JobName} failed.", jobName);
            throw new JobExecutionException(ex, refireImmediately: false);
        }
    }

    protected abstract Task ExecuteAsync(CancellationToken ct);
}
