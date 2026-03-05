namespace BuildingBlocks.Persistence.EfCore.Outbox;

public class OutboxCleanerOptions
{
    public int PollingIntervalSeconds { get; set; } = 3600;
    public int BatchSize             { get; set; } = 1000;
    public int RetentionDays         { get; set; } = 7;
    public int MaxRetryCount         { get; set; } = 3;
}
