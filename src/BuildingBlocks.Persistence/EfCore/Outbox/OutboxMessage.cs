namespace BuildingBlocks.Persistence.EfCore.Outbox;

public class OutboxMessage
{
    public Guid            Id           { get; set; }
    public required string EventType    { get; set; }
    public required string Payload      { get; set; }
    public DateTimeOffset  OccurredOn   { get; set; }
    public DateTimeOffset? ProcessedOn  { get; set; }
    public DateTimeOffset? LastErrorOn     { get; set; }
    public string?         ErrorMessage { get; set; }
    public int             RetryCount   { get; set; }
}
