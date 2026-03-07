namespace BuildingBlocks.Infrastructure.Jobs;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CronJobAttribute : Attribute
{
    public string CronExpression { get; }
    public string? Identity { get; }
    public bool DisallowConcurrentExecution { get; set; }

    public CronJobAttribute(string cronExpression, string? identity = null)
    {
        CronExpression = cronExpression;
        Identity = identity;
    }
}
