using BuildingBlocks.Application.Csv;
using BuildingBlocks.Infrastructure.Csv;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Csv;

public class CsvFileWriterTests
{
    private readonly CsvFileWriter _writer = new();

    [Fact]
    public void ImplementsICsvWriter()
    {
        _writer.Should().BeAssignableTo<ICsvWriter>();
    }

    [Fact]
    public async Task WriteAsync_WritesHeaderAndRows()
    {
        var records = new[]
        {
            new TestRecord { Name = "Ali", Age = 30 },
            new TestRecord { Name = "Veli", Age = 25 }
        };

        using var stream = new MemoryStream();
        await _writer.WriteAsync(records, stream);

        var csv = await ReadStreamAsString(stream);
        var lines = csv.TrimEnd().Split('\n');

        lines[0].Trim().Should().Be("Name,Age");
        lines[1].Trim().Should().Be("Ali,30");
        lines[2].Trim().Should().Be("Veli,25");
    }

    [Fact]
    public async Task WriteAsync_EmptyCollection_WritesOnlyHeader()
    {
        var records = Array.Empty<TestRecord>();

        using var stream = new MemoryStream();
        await _writer.WriteAsync(records, stream);

        var csv = await ReadStreamAsString(stream);
        var lines = csv.TrimEnd().Split('\n');

        lines.Should().HaveCount(1);
        lines[0].Trim().Should().Be("Name,Age");
    }

    [Fact]
    public async Task WriteAsync_LeavesStreamOpen()
    {
        using var stream = new MemoryStream();
        await _writer.WriteAsync(Array.Empty<TestRecord>(), stream);

        stream.CanRead.Should().BeTrue();
    }

    private static async Task<string> ReadStreamAsString(MemoryStream stream)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}

public record TestRecord
{
    public string Name { get; init; } = "";
    public int Age { get; init; }
}
