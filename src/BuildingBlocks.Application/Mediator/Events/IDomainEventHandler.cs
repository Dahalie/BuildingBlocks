using MediatR;

namespace BuildingBlocks.Application.Mediator.Events;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent;