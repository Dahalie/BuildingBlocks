using BuildingBlocks.Application.Messaging;
using FluentAssertions;

namespace BuildingBlocks.Application.Tests.Messaging;

public record TestEvent(string Data) : IntegrationEventBase;

public class IntegrationEventBaseTests
{
    [Fact]
    public void IntegrationEventBase_ImplementsIIntegrationEvent()
    {
        var evt = new TestEvent("test");

        evt.Should().BeAssignableTo<IIntegrationEvent>();
    }

    [Fact]
    public void Properties_AreSettable()
    {
        var messageId = Guid.NewGuid();
        var occurredOn = DateTimeOffset.UtcNow;

        var evt = new TestEvent("data")
        {
            MessageId  = messageId,
            OccurredOn = occurredOn
        };

        evt.MessageId.Should().Be(messageId);
        evt.OccurredOn.Should().Be(occurredOn);
        evt.Data.Should().Be("data");
    }
}
