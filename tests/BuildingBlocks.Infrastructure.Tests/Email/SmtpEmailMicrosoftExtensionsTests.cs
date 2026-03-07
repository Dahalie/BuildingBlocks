using BuildingBlocks.Application.Email;
using BuildingBlocks.Infrastructure.Email;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Tests.Email;

public class SmtpEmailMicrosoftExtensionsTests
{
    [Fact]
    public void AddSmtpEmail_WithOptions_RegistersServices()
    {
        var services = new ServiceCollection();

        var options = new SmtpOptions
        {
            Host = "smtp.example.com",
            SenderAddress = "no-reply@example.com"
        };

        services.AddSmtpEmail(options);

        var provider = services.BuildServiceProvider();

        provider.GetService<SmtpOptions>().Should().BeSameAs(options);
        provider.GetService<IEmailSender>().Should().NotBeNull();
    }

    [Fact]
    public void AddSmtpEmail_WithConfigSection_BindsOptions()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Smtp:Host"] = "mail.example.com",
                ["Smtp:Port"] = "465",
                ["Smtp:SenderAddress"] = "test@example.com",
                ["Smtp:SenderName"] = "Test",
                ["Smtp:UseSsl"] = "false"
            })
            .Build();

        services.AddSmtpEmail(config.GetSection("Smtp"));

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<SmtpOptions>();

        resolved.Host.Should().Be("mail.example.com");
        resolved.Port.Should().Be(465);
        resolved.SenderAddress.Should().Be("test@example.com");
        resolved.SenderName.Should().Be("Test");
        resolved.UseSsl.Should().BeFalse();
    }

    [Fact]
    public void AddSmtpEmail_WithConfigureCallback_OverridesConfig()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Smtp:Host"] = "original.example.com",
                ["Smtp:SenderAddress"] = "original@example.com"
            })
            .Build();

        services.AddSmtpEmail(
            config.GetSection("Smtp"),
            opts => opts.Host = "overridden.example.com");

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<SmtpOptions>();

        resolved.Host.Should().Be("overridden.example.com");
    }

    [Fact]
    public void AddSmtpEmail_ReturnsServiceCollection_ForChaining()
    {
        var services = new ServiceCollection();

        var options = new SmtpOptions
        {
            Host = "smtp.example.com",
            SenderAddress = "no-reply@example.com"
        };

        var result = services.AddSmtpEmail(options);

        result.Should().BeSameAs(services);
    }
}
