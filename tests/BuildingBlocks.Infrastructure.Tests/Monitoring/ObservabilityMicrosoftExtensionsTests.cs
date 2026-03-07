using BuildingBlocks.Infrastructure.Monitoring;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Infrastructure.Tests.Monitoring;

public class ObservabilityMicrosoftExtensionsTests
{
    [Fact]
    public void AddObservability_WithOptions_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var options = new ObservabilityOptions
        {
            ServiceName = "TestService",
            EnableTracing = true,
            EnableMetrics = true
        };

        services.AddObservability(options);

        var provider = services.BuildServiceProvider();
        var tracerProvider = provider.GetService<TracerProvider>();

        tracerProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddObservability_WithConfigSection_BindsOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Observability:ServiceName"] = "ConfiguredService",
                ["Observability:EnableTracing"] = "true",
                ["Observability:EnableMetrics"] = "false"
            })
            .Build();

        services.AddObservability(config.GetSection("Observability"));

        var provider = services.BuildServiceProvider();
        var tracerProvider = provider.GetService<TracerProvider>();

        tracerProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddObservability_WithConfigureCallback_OverridesConfig()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Observability:ServiceName"] = "FromConfig"
            })
            .Build();

        ObservabilityOptions? capturedOptions = null;

        services.AddObservability(
            config.GetSection("Observability"),
            opts =>
            {
                capturedOptions = opts;
                opts.ServiceName = "Overridden";
            });

        capturedOptions.Should().NotBeNull();
        capturedOptions!.ServiceName.Should().Be("Overridden");
    }

    [Fact]
    public void AddObservability_TracingDisabled_StillSucceeds()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var options = new ObservabilityOptions
        {
            EnableTracing = false,
            EnableMetrics = false
        };

        var act = () => services.AddObservability(options);

        act.Should().NotThrow();
    }

    [Fact]
    public void ResolveOtlpEndpoint_WithOptionsEndpoint_ReturnsUri()
    {
        var options = new ObservabilityOptions { OtlpEndpoint = "http://localhost:4317" };

        var result = ObservabilityMicrosoftExtensions.ResolveOtlpEndpoint(options);

        result.Should().NotBeNull();
        result!.ToString().Should().Be("http://localhost:4317/");
    }

    [Fact]
    public void ResolveOtlpEndpoint_WithNullEndpoint_ReturnsNull()
    {
        var options = new ObservabilityOptions { OtlpEndpoint = null };

        // Clear env var to ensure clean test
        var originalEnvVar = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);

            var result = ObservabilityMicrosoftExtensions.ResolveOtlpEndpoint(options);

            result.Should().BeNull();
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", originalEnvVar);
        }
    }
}
