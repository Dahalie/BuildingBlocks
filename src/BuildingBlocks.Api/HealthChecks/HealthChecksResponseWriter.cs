using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Api.HealthChecks;

public static class HealthChecksResponseWriter
{
    public static async Task WriteAsync(HttpContext httpContext, HealthReport report)
    {
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            writer.WriteString("status", report.Status.ToString());
            writer.WriteString("totalDuration", report.TotalDuration.ToString());

            writer.WriteStartArray("checks");

            foreach (var (name, entry) in report.Entries)
            {
                writer.WriteStartObject();
                writer.WriteString("name", name);
                writer.WriteString("status", entry.Status.ToString());
                writer.WriteString("duration", entry.Duration.ToString());

                if (!string.IsNullOrWhiteSpace(entry.Description))
                    writer.WriteString("description", entry.Description);

                if (entry.Exception is not null)
                    writer.WriteString("exception", entry.Exception.Message);

                if (entry.Data.Count > 0)
                {
                    writer.WriteStartObject("data");
                    foreach (var (key, value) in entry.Data)
                        writer.WriteString(key, value?.ToString());
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        await httpContext.Response.WriteAsync(System.Text.Encoding.UTF8.GetString(stream.ToArray()));
    }
}
