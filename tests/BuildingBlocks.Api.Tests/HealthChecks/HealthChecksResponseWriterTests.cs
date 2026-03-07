using System.Text.Json;
using BuildingBlocks.Api.HealthChecks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Api.Tests.HealthChecks;

public class HealthChecksResponseWriterTests
{
    [Fact]
    public async Task WriteAsync_HealthyReport_WritesJsonWithStatus()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["test-check"] = new(HealthStatus.Healthy, "All good", TimeSpan.FromMilliseconds(50), null, null)
            },
            TimeSpan.FromMilliseconds(100));

        await HealthChecksResponseWriter.WriteAsync(httpContext, report);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var doc = await JsonDocument.ParseAsync(httpContext.Response.Body);
        var root = doc.RootElement;

        root.GetProperty("status").GetString().Should().Be("Healthy");
        root.GetProperty("checks").GetArrayLength().Should().Be(1);
        root.GetProperty("checks")[0].GetProperty("name").GetString().Should().Be("test-check");
        root.GetProperty("checks")[0].GetProperty("status").GetString().Should().Be("Healthy");
        root.GetProperty("checks")[0].GetProperty("description").GetString().Should().Be("All good");
    }

    [Fact]
    public async Task WriteAsync_SetsContentTypeToJson()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), TimeSpan.Zero);

        await HealthChecksResponseWriter.WriteAsync(httpContext, report);

        httpContext.Response.ContentType.Should().Be("application/json; charset=utf-8");
    }

    [Fact]
    public async Task WriteAsync_UnhealthyReport_IncludesExceptionMessage()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var exception = new InvalidOperationException("Connection failed");
        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["db"] = new(HealthStatus.Unhealthy, "DB down", TimeSpan.FromSeconds(1), exception, null)
            },
            TimeSpan.FromSeconds(1));

        await HealthChecksResponseWriter.WriteAsync(httpContext, report);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var doc = await JsonDocument.ParseAsync(httpContext.Response.Body);
        var check = doc.RootElement.GetProperty("checks")[0];

        check.GetProperty("status").GetString().Should().Be("Unhealthy");
        check.GetProperty("exception").GetString().Should().Be("Connection failed");
    }

    [Fact]
    public async Task WriteAsync_EmptyReport_WritesEmptyChecksArray()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), TimeSpan.Zero);

        await HealthChecksResponseWriter.WriteAsync(httpContext, report);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var doc = await JsonDocument.ParseAsync(httpContext.Response.Body);

        doc.RootElement.GetProperty("checks").GetArrayLength().Should().Be(0);
    }
}
