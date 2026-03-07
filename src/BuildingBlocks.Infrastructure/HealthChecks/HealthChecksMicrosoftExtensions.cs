using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Infrastructure.HealthChecks;

public static class HealthChecksMicrosoftExtensions
{
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services, Action<IHealthChecksBuilder>? configure = null)
    {
        var builder = services.AddHealthChecks();
        configure?.Invoke(builder);
        return services;
    }

    public static IHealthChecksBuilder AddHttpHealthCheck(this IHealthChecksBuilder builder, string name, Uri uri,
        HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null)
    {
        var allTags = MergeWithReadinessTag(tags);
        return builder.AddCheck(name, new HttpHealthCheck(uri), failureStatus, allTags, timeout);
    }

    public static IHealthChecksBuilder AddTcpHealthCheck(this IHealthChecksBuilder builder, string name, string host, int port,
        HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null)
    {
        var allTags = MergeWithReadinessTag(tags);
        return builder.AddCheck(name, new TcpHealthCheck(host, port), failureStatus, allTags, timeout);
    }

    private static IEnumerable<string> MergeWithReadinessTag(IEnumerable<string>? tags)
    {
        var tagList = tags?.ToList() ?? [];

        if (!tagList.Contains(HealthCheckTags.Readiness))
            tagList.Add(HealthCheckTags.Readiness);

        return tagList;
    }
}
