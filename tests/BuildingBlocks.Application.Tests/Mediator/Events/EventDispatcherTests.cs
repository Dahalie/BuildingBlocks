using BuildingBlocks.Application.Mediator.Events;
using FluentAssertions;
using NSubstitute;

namespace BuildingBlocks.Application.Tests.Mediator.Events;

public record TestDomainEvent : DomainEventBase;

public class EventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_PublishesViaMediatR()
    {
        var publisher = Substitute.For<MediatR.IPublisher>();
        IEventDispatcher dispatcher = new EventDispatcher(publisher);
        var domainEvent = new TestDomainEvent();

        await dispatcher.DispatchAsync(domainEvent, CancellationToken.None);

        await publisher.Received(1).Publish(domainEvent, CancellationToken.None);
    }

    [Fact]
    public void DomainEventBase_ImplementsIDomainEvent()
    {
        var evt = new TestDomainEvent();

        evt.Should().BeAssignableTo<IDomainEvent>();
    }
}
