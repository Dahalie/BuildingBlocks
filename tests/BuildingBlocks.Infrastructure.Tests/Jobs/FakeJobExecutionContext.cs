using Quartz;

namespace BuildingBlocks.Infrastructure.Tests.Jobs;

internal sealed class FakeJobExecutionContext : IJobExecutionContext
{
    public IScheduler Scheduler => null!;
    public ITrigger Trigger => null!;
    public ICalendar? Calendar => null;
    public bool Recovering => false;
    public TriggerKey RecoveringTriggerKey => null!;
    public int RefireCount => 0;
    public JobDataMap MergedJobDataMap => new();
    public IJobDetail JobDetail => null!;
    public IJob JobInstance => null!;
    public DateTimeOffset FireTimeUtc => DateTimeOffset.UtcNow;
    public DateTimeOffset? ScheduledFireTimeUtc => null;
    public DateTimeOffset? PreviousFireTimeUtc => null;
    public DateTimeOffset? NextFireTimeUtc => null;
    public string FireInstanceId => "test";
    public object? Result { get; set; }
    public TimeSpan JobRunTime => TimeSpan.Zero;
    public CancellationToken CancellationToken => CancellationToken.None;

    public void Put(object key, object objectValue) { }
    public object? Get(object key) => null;
}
