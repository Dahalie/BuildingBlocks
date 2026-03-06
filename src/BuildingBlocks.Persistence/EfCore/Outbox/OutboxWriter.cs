using System.Text.Json;
using BuildingBlocks.Application.Clock;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Persistence.EfCore.Outbox;

public class OutboxWriter(IDateTimeProvider dateTimeProvider) : IOutboxWriter
{
    private readonly List<OutboxMessage> _stagedMessages = [];

    public void Stage(IIntegrationEvent integrationEvent)
    {
        if (integrationEvent is IntegrationEventBase eventBase)
        {
            eventBase.MessageId  = Guid.CreateVersion7();
            eventBase.OccurredOn = dateTimeProvider.UtcNow;
        }

        var message = new OutboxMessage
        {
            Id         = Guid.CreateVersion7(),
            EventType  = integrationEvent.GetType().FullName!,
            Payload    = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType()),
            OccurredOn = integrationEvent.OccurredOn
        };

        _stagedMessages.Add(message);
    }

    internal IReadOnlyList<OutboxMessage> GetStagedMessages() => _stagedMessages;

    internal void Clear() => _stagedMessages.Clear();
}
