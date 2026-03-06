using BuildingBlocks.Api.Identity;
using BuildingBlocks.Application.Identity;
using FluentAssertions;

namespace BuildingBlocks.Api.Tests.Identity;

public class FakeCurrentUserProviderTests
{
    [Fact]
    public void UserId_ReturnsFixedGuid()
    {
        var provider = new FakeCurrentUserProvider();

        provider.UserId.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    }

    [Fact]
    public void ImplementsICurrentUserProvider()
    {
        var provider = new FakeCurrentUserProvider();

        provider.Should().BeAssignableTo<ICurrentUserProvider>();
    }

    [Fact]
    public void UserId_IsConsistentAcrossCalls()
    {
        var provider = new FakeCurrentUserProvider();

        provider.UserId.Should().Be(provider.UserId);
    }
}
