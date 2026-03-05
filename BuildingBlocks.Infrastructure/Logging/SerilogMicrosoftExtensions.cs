using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Infrastructure.Logging;

public static class SerilogMicrosoftExtensions
{
    public static IHostBuilder UseSerilogDefaults(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext().Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning).MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information).MinimumLevel
                         .Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning).WriteTo
                         .Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}");
        });
    }
}