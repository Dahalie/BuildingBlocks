using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Api.HealthChecks;

public static class HealthCheckEndpointExtensions
{
    private const string LiveTag = "live";
    private const string ReadyTag = "ready";

    public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder app, string pattern = "/health")
    {
        app.MapHealthChecks(pattern, CreateOptions());
        return app;
    }

    public static IEndpointRouteBuilder MapLivenessHealthChecks(this IEndpointRouteBuilder app, string pattern = "/health/live")
    {
        app.MapHealthChecks(pattern, CreateOptions(entry => entry.Tags.Contains(LiveTag)));
        return app;
    }

    public static IEndpointRouteBuilder MapReadinessHealthChecks(this IEndpointRouteBuilder app, string pattern = "/health/ready")
    {
        app.MapHealthChecks(pattern, CreateOptions(entry => entry.Tags.Contains(ReadyTag)));
        return app;
    }

    private static HealthCheckOptions CreateOptions(Func<HealthCheckRegistration, bool>? predicate = null)
    {
        return new HealthCheckOptions
        {
            Predicate = predicate,
            ResponseWriter = HealthChecksResponseWriter.WriteAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            }
        };
    }
}
