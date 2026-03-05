namespace BuildingBlocks.Persistence.EfCore.Inbox;

public class InboxMessage
{
    public Guid            Id           { get; set; }
    public Guid            MessageId    { get; set; }
    public required string EventType    { get; set; }
    public required string Payload      { get; set; }
    public DateTimeOffset  ReceivedOn   { get; set; }
    public DateTimeOffset? ProcessedOn  { get; set; }
    public required string ConsumerType { get; set; }
    public string?         ErrorMessage { get; set; }
}
