using BuildingBlocks.Application.Clock;
using BuildingBlocks.Infrastructure.Clock;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Clock;

public class DateTimeProviderTests
{
    [Fact]
    public void UtcNow_ReturnsCurrentUtcTime()
    {
        var provider = new DateTimeProvider();
        var before = DateTimeOffset.UtcNow;

        var result = provider.UtcNow;

        var after = DateTimeOffset.UtcNow;
        result.Should().BeOnOrAfter(before);
        result.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void ImplementsIDateTimeProvider()
    {
        var provider = new DateTimeProvider();

        provider.Should().BeAssignableTo<IDateTimeProvider>();
    }
}
