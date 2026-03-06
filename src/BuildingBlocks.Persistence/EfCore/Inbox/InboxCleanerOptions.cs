namespace BuildingBlocks.Persistence.EfCore.Inbox;

public class InboxCleanerOptions
{
    public int PollingIntervalSeconds { get; set; } = 3600;
    public int BatchSize             { get; set; } = 1000;
    public int RetentionDays         { get; set; } = 7;
}
