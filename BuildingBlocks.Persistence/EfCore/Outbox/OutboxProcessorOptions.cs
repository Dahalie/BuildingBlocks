namespace BuildingBlocks.Persistence.EfCore.Outbox;

public class OutboxProcessorOptions
{
    public int PollingIntervalSeconds { get; set; } = 10;
    public int BatchSize             { get; set; } = 20;
    public int MaxRetryCount         { get; set; } = 3;
}
