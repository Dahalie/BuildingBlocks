using BuildingBlocks.Application.Email;
using FluentAssertions;

namespace BuildingBlocks.Application.Tests.Email;

public class EmailAddressTests
{
    [Fact]
    public void Constructor_WithAddressOnly_SetsProperties()
    {
        var address = new EmailAddress("test@example.com");

        address.Address.Should().Be("test@example.com");
        address.Name.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithAddressAndName_SetsProperties()
    {
        var address = new EmailAddress("test@example.com", "Test User");

        address.Address.Should().Be("test@example.com");
        address.Name.Should().Be("Test User");
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new EmailAddress("test@example.com", "Test");
        var b = new EmailAddress("test@example.com", "Test");

        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var a = new EmailAddress("a@example.com");
        var b = new EmailAddress("b@example.com");

        a.Should().NotBe(b);
    }
}
