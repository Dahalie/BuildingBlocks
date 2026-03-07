using BuildingBlocks.Infrastructure.Monitoring;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Monitoring;

public class ObservabilityOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new ObservabilityOptions();

        options.ServiceName.Should().Be("UnknownService");
        options.ServiceVersion.Should().BeNull();
        options.OtlpEndpoint.Should().BeNull();
        options.EnableTracing.Should().BeTrue();
        options.EnableMetrics.Should().BeTrue();
        options.EnableLogging.Should().BeTrue();
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var options = new ObservabilityOptions
        {
            ServiceName = "MyService",
            ServiceVersion = "2.0.0",
            OtlpEndpoint = "http://collector:4317",
            EnableTracing = false,
            EnableMetrics = false,
            EnableLogging = false
        };

        options.ServiceName.Should().Be("MyService");
        options.ServiceVersion.Should().Be("2.0.0");
        options.OtlpEndpoint.Should().Be("http://collector:4317");
        options.EnableTracing.Should().BeFalse();
        options.EnableMetrics.Should().BeFalse();
        options.EnableLogging.Should().BeFalse();
    }
}
