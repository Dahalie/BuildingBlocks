using System.Net.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Infrastructure.HealthChecks;

public class HttpHealthCheck(Uri uri) : IHealthCheck
{
    private static readonly HttpClient HttpClient = new();

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await HttpClient.GetAsync(uri, cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"HTTP endpoint {uri} is reachable.")
                : new HealthCheckResult(context.Registration.FailureStatus, $"HTTP endpoint {uri} returned status code {response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, $"HTTP endpoint {uri} is unreachable.", ex);
        }
    }
}
