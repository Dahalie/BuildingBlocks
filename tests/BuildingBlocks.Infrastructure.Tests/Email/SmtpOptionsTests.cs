using BuildingBlocks.Infrastructure.Email;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Email;

public class SmtpOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new SmtpOptions
        {
            Host = "smtp.example.com",
            SenderAddress = "no-reply@example.com"
        };

        options.Port.Should().Be(587);
        options.UseSsl.Should().BeTrue();
        options.Username.Should().BeNull();
        options.Password.Should().BeNull();
        options.SenderName.Should().BeNull();
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        var options = new SmtpOptions
        {
            Host = "mail.example.com",
            Port = 465,
            Username = "user",
            Password = "pass",
            UseSsl = false,
            SenderAddress = "sender@example.com",
            SenderName = "My App"
        };

        options.Host.Should().Be("mail.example.com");
        options.Port.Should().Be(465);
        options.Username.Should().Be("user");
        options.Password.Should().Be("pass");
        options.UseSsl.Should().BeFalse();
        options.SenderAddress.Should().Be("sender@example.com");
        options.SenderName.Should().Be("My App");
    }
}
