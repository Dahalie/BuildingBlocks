namespace BuildingBlocks.Contracts.Messaging;

public interface IIntegrationEvent
{
    Guid           MessageId  { get; }
    DateTimeOffset OccurredOn { get; }
}
