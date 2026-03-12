using MediatR;

namespace BuildingBlocks.Application.Mediator.Events;

internal sealed class EventDispatcher(IPublisher publisher) : IEventDispatcher
{
    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        => publisher.Publish(domainEvent, cancellationToken);
}