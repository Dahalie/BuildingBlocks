using System.Text;
using BuildingBlocks.Application.Csv;
using BuildingBlocks.Infrastructure.Csv;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Csv;

public class CsvFileReaderTests
{
    private readonly CsvFileReader _reader = new();

    [Fact]
    public void ImplementsICsvReader()
    {
        _reader.Should().BeAssignableTo<ICsvReader>();
    }

    [Fact]
    public async Task ReadAsync_ParsesHeaderAndRows()
    {
        using var stream = CreateStream("Name,Age\nAli,30\nVeli,25\n");

        var records = await _reader.ReadAsync<TestRecord>(stream);

        records.Should().HaveCount(2);
        records[0].Name.Should().Be("Ali");
        records[0].Age.Should().Be(30);
        records[1].Name.Should().Be("Veli");
        records[1].Age.Should().Be(25);
    }

    [Fact]
    public async Task ReadAsync_EmptyCsv_ReturnsEmptyList()
    {
        using var stream = CreateStream("Name,Age\n");

        var records = await _reader.ReadAsync<TestRecord>(stream);

        records.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadAsync_LeavesStreamOpen()
    {
        using var stream = CreateStream("Name,Age\n");

        await _reader.ReadAsync<TestRecord>(stream);

        stream.CanRead.Should().BeTrue();
    }

    [Fact]
    public async Task Roundtrip_WriteAndRead_PreservesData()
    {
        var original = new[]
        {
            new TestRecord { Name = "Ali", Age = 30 },
            new TestRecord { Name = "Veli", Age = 25 }
        };

        using var stream = new MemoryStream();
        var writer = new CsvFileWriter();
        await writer.WriteAsync(original, stream);

        stream.Position = 0;
        var result = await _reader.ReadAsync<TestRecord>(stream);

        result.Should().BeEquivalentTo(original);
    }

    private static MemoryStream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}
