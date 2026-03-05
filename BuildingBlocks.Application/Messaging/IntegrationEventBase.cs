namespace BuildingBlocks.Application.Messaging;

public abstract record IntegrationEventBase : IIntegrationEvent
{
    public Guid           MessageId  { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
}