using BuildingBlocks.Infrastructure.HealthChecks;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Infrastructure.Tests.HealthChecks;

public class HttpHealthCheckTests
{
    private static HealthCheckContext CreateContext(HealthStatus failureStatus = HealthStatus.Unhealthy)
    {
        return new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", new HttpHealthCheck(new Uri("http://localhost")), failureStatus, null)
        };
    }

    [Fact]
    public void ImplementsIHealthCheck()
    {
        var check = new HttpHealthCheck(new Uri("http://localhost"));

        check.Should().BeAssignableTo<IHealthCheck>();
    }

    [Fact]
    public async Task CheckHealthAsync_UnreachableEndpoint_ReturnsUnhealthy()
    {
        var check = new HttpHealthCheck(new Uri("http://localhost:19999"));
        var context = CreateContext();

        var result = await check.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Exception.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckHealthAsync_UnreachableEndpoint_UsesConfiguredFailureStatus()
    {
        var check = new HttpHealthCheck(new Uri("http://localhost:19999"));
        var context = CreateContext(HealthStatus.Degraded);

        var result = await check.CheckHealthAsync(context);

        result.Status.Should().Be(HealthStatus.Degraded);
    }
}
