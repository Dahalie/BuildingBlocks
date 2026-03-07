using BuildingBlocks.Infrastructure.HealthChecks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Tests.HealthChecks;

public class HealthChecksMicrosoftExtensionsTests
{
    [Fact]
    public void AddDefaultHealthChecks_RegistersHealthCheckService()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddDefaultHealthChecks();

        var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetService<HealthCheckService>();

        healthCheckService.Should().NotBeNull();
    }

    [Fact]
    public void AddDefaultHealthChecks_WithConfigure_InvokesCallback()
    {
        var services = new ServiceCollection();
        var callbackInvoked = false;

        services.AddDefaultHealthChecks(builder =>
        {
            callbackInvoked = true;
        });

        callbackInvoked.Should().BeTrue();
    }

    [Fact]
    public void AddHttpHealthCheck_RegistersCheckWithReadinessTag()
    {
        var services = new ServiceCollection();

        services.AddDefaultHealthChecks(builder =>
        {
            builder.AddHttpHealthCheck("test-http", new Uri("http://localhost"));
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.Should().ContainSingle(r => r.Name == "test-http").Subject;

        registration.Tags.Should().Contain(HealthCheckTags.Readiness);
    }

    [Fact]
    public void AddTcpHealthCheck_RegistersCheckWithReadinessTag()
    {
        var services = new ServiceCollection();

        services.AddDefaultHealthChecks(builder =>
        {
            builder.AddTcpHealthCheck("test-tcp", "localhost", 5432);
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.Should().ContainSingle(r => r.Name == "test-tcp").Subject;

        registration.Tags.Should().Contain(HealthCheckTags.Readiness);
    }

    [Fact]
    public void AddHttpHealthCheck_PreservesCustomTags()
    {
        var services = new ServiceCollection();

        services.AddDefaultHealthChecks(builder =>
        {
            builder.AddHttpHealthCheck("test", new Uri("http://localhost"), tags: ["custom-tag"]);
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.Single(r => r.Name == "test");

        registration.Tags.Should().Contain("custom-tag");
        registration.Tags.Should().Contain(HealthCheckTags.Readiness);
    }
}
