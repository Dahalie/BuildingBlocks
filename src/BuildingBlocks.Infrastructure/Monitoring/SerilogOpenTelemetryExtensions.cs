using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace BuildingBlocks.Infrastructure.Monitoring;

public static class SerilogOpenTelemetryExtensions
{
    public static IHostBuilder UseSerilogWithObservability(
        this IHostBuilder hostBuilder,
        IConfigurationSection configSection)
    {
        var options = new ObservabilityOptions();
        configSection.Bind(options);

        return UseSerilogWithObservability(hostBuilder, options);
    }

    public static IHostBuilder UseSerilogWithObservability(
        this IHostBuilder hostBuilder,
        ObservabilityOptions options)
    {
        var otlpEndpoint = ObservabilityMicrosoftExtensions.ResolveOtlpEndpoint(options);

        return hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}");

            if (options.EnableLogging && otlpEndpoint is not null)
            {
                configuration.WriteTo.OpenTelemetry(otelOptions =>
                {
                    otelOptions.Endpoint = otlpEndpoint.ToString();
                    otelOptions.Protocol = OtlpProtocol.Grpc;
                    otelOptions.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = options.ServiceName
                    };
                });
            }
        });
    }
}
