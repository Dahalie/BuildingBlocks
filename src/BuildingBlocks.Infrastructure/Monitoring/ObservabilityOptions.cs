namespace BuildingBlocks.Infrastructure.Monitoring;

public class ObservabilityOptions
{
    public string ServiceName { get; set; } = "UnknownService";
    public string? ServiceVersion { get; set; }
    public string? OtlpEndpoint { get; set; }
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
}
