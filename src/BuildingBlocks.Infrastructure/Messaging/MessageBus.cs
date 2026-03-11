using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Contracts.Messaging;
using MassTransit;

namespace BuildingBlocks.Infrastructure.Messaging;

public class MessageBus(IPublishEndpoint publishEndpoint) : IMessageBus
{
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IIntegrationEvent
    {
        await publishEndpoint.Publish(message, cancellationToken);
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        await publishEndpoint.Publish(message, cancellationToken);
    }
}