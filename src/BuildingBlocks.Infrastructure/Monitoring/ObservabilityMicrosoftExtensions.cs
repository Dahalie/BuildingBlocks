using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Infrastructure.Monitoring;

public static class ObservabilityMicrosoftExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfigurationSection configSection,
        Action<ObservabilityOptions>? configure = null)
    {
        var options = new ObservabilityOptions();
        configSection.Bind(options);
        configure?.Invoke(options);

        return AddObservability(services, options);
    }

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        ObservabilityOptions options)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: options.ServiceName,
                serviceVersion: options.ServiceVersion);

        var otlpEndpoint = ResolveOtlpEndpoint(options);

        var otel = services.AddOpenTelemetry();

        if (options.EnableTracing)
        {
            otel.WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("Microsoft.EntityFrameworkCore")
                    .AddSource("MassTransit");

                if (otlpEndpoint is not null)
                    tracing.AddOtlpExporter(o => o.Endpoint = otlpEndpoint);
            });
        }

        if (options.EnableMetrics)
        {
            otel.WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (otlpEndpoint is not null)
                    metrics.AddOtlpExporter(o => o.Endpoint = otlpEndpoint);
            });
        }

        return services;
    }

    internal static Uri? ResolveOtlpEndpoint(ObservabilityOptions options)
    {
        var endpoint = options.OtlpEndpoint
                       ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

        return !string.IsNullOrEmpty(endpoint) ? new Uri(endpoint) : null;
    }
}
