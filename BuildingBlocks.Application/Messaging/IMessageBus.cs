namespace BuildingBlocks.Application.Messaging;

public interface IMessageBus
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IIntegrationEvent;

    Task PublishAsync(object message, CancellationToken cancellationToken = default);
}