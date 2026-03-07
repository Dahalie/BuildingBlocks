using BuildingBlocks.Infrastructure.HealthChecks;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Infrastructure.Tests.HealthChecks;

public class TcpHealthCheckTests
{
    private static HealthCheckContext CreateContext(HealthStatus failureStatus = HealthStatus.Unhealthy)
    {
        return new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", new TcpHealthCheck("localhost", 19999), failureStatus, null)
        };
    }

    [Fact]
    public void ImplementsIHealthCheck()
    {
        var check = new TcpHealthCheck("localhost", 80);

        check.Should().BeAssignableTo<IHealthCheck>();
    }

    [Fact]
    public async Task CheckHealthAsync_UnreachablePort_ReturnsUnhealthy()
    {
        var check = new TcpHealthCheck("localhost", 19999);
        var context = CreateContext();

        var result = await check.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Exception.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckHealthAsync_UnreachablePort_UsesConfiguredFailureStatus()
    {
        var check = new TcpHealthCheck("localhost", 19999);
        var context = CreateContext(HealthStatus.Degraded);

        var result = await check.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Degraded);
    }

    [Fact]
    public async Task CheckHealthAsync_UnreachablePort_IncludesHostAndPortInDescription()
    {
        var check = new TcpHealthCheck("localhost", 19999);
        var context = CreateContext();

        var result = await check.CheckHealthAsync(context);

        result.Description.Should().Contain("localhost:19999");
    }
}
