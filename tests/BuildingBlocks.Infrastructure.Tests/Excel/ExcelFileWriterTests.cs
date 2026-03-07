using BuildingBlocks.Application.Excel;
using BuildingBlocks.Infrastructure.Excel;
using ClosedXML.Excel;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Excel;

public class ExcelFileWriterTests
{
    private readonly ExcelFileWriter _writer = new();

    [Fact]
    public void ImplementsIExcelWriter()
    {
        _writer.Should().BeAssignableTo<IExcelWriter>();
    }

    [Fact]
    public async Task WriteAsync_WritesHeaderAndRows()
    {
        var records = new[]
        {
            new ExcelTestRecord { Name = "Ali", Age = 30, Salary = 5000m },
            new ExcelTestRecord { Name = "Veli", Age = 25, Salary = 4000m }
        };

        using var stream = new MemoryStream();
        await _writer.WriteAsync(records, stream);

        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheets.First();

        ws.Cell(1, 1).GetString().Should().Be("Name");
        ws.Cell(1, 2).GetString().Should().Be("Age");
        ws.Cell(1, 3).GetString().Should().Be("Salary");
        ws.Cell(2, 1).GetString().Should().Be("Ali");
        ws.Cell(2, 2).GetDouble().Should().Be(30);
        ws.Cell(3, 1).GetString().Should().Be("Veli");
    }

    [Fact]
    public async Task WriteAsync_CustomSheetName()
    {
        using var stream = new MemoryStream();
        await _writer.WriteAsync(Array.Empty<ExcelTestRecord>(), stream, new ExcelWriteOptions { SheetName = "Rapor" });

        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);

        workbook.Worksheets.First().Name.Should().Be("Rapor");
    }

    [Fact]
    public async Task WriteSheetsAsync_WritesMultipleSheets()
    {
        var sheets = new Dictionary<string, IEnumerable<ExcelTestRecord>>
        {
            ["Istanbul"] = [new ExcelTestRecord { Name = "Ali", Age = 30, Salary = 5000m }],
            ["Ankara"] = [new ExcelTestRecord { Name = "Veli", Age = 25, Salary = 4000m }]
        };

        using var stream = new MemoryStream();
        await _writer.WriteSheetsAsync(sheets, stream);

        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);

        workbook.Worksheets.Should().HaveCount(2);
        workbook.Worksheet("Istanbul").Cell(2, 1).GetString().Should().Be("Ali");
        workbook.Worksheet("Ankara").Cell(2, 1).GetString().Should().Be("Veli");
    }

    [Fact]
    public async Task WriteAsync_EmptyCollection_WritesOnlyHeader()
    {
        using var stream = new MemoryStream();
        await _writer.WriteAsync(Array.Empty<ExcelTestRecord>(), stream);

        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheets.First();

        ws.Cell(1, 1).GetString().Should().Be("Name");
        ws.LastRowUsed()!.RowNumber().Should().Be(1);
    }
}
