using BuildingBlocks.Application.Excel;
using BuildingBlocks.Infrastructure.Excel;
using ClosedXML.Excel;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Excel;

public class ExcelFileReaderTests
{
    private readonly ExcelFileReader _reader = new();

    [Fact]
    public void ImplementsIExcelReader()
    {
        _reader.Should().BeAssignableTo<IExcelReader>();
    }

    [Fact]
    public async Task GetSheetNamesAsync_ReturnsAllSheetNames()
    {
        using var stream = CreateExcelStream(wb =>
        {
            wb.Worksheets.Add("Sheet1");
            wb.Worksheets.Add("Sheet2");
            wb.Worksheets.Add("Rapor");
        });

        var names = await _reader.GetSheetNamesAsync(stream);

        names.Should().BeEquivalentTo(["Sheet1", "Sheet2", "Rapor"]);
    }

    [Fact]
    public async Task ReadAsync_ParsesHeaderAndRows()
    {
        using var stream = CreateExcelStream(wb =>
        {
            var ws = wb.Worksheets.Add("Sheet1");
            ws.Cell(1, 1).Value = "Name";
            ws.Cell(1, 2).Value = "Age";
            ws.Cell(1, 3).Value = "Salary";
            ws.Cell(2, 1).Value = "Ali";
            ws.Cell(2, 2).Value = 30;
            ws.Cell(2, 3).Value = 5000;
            ws.Cell(3, 1).Value = "Veli";
            ws.Cell(3, 2).Value = 25;
            ws.Cell(3, 3).Value = 4000;
        });

        var records = await _reader.ReadAsync<ExcelTestRecord>(stream);

        records.Should().HaveCount(2);
        records[0].Name.Should().Be("Ali");
        records[0].Age.Should().Be(30);
        records[0].Salary.Should().Be(5000m);
        records[1].Name.Should().Be("Veli");
    }

    [Fact]
    public async Task ReadAsync_WithSheetName_ReadsSpecificSheet()
    {
        using var stream = CreateExcelStream(wb =>
        {
            var ws1 = wb.Worksheets.Add("First");
            ws1.Cell(1, 1).Value = "Name";
            ws1.Cell(1, 2).Value = "Age";
            ws1.Cell(1, 3).Value = "Salary";
            ws1.Cell(2, 1).Value = "Ali";
            ws1.Cell(2, 2).Value = 30;
            ws1.Cell(2, 3).Value = 5000;

            var ws2 = wb.Worksheets.Add("Second");
            ws2.Cell(1, 1).Value = "Name";
            ws2.Cell(1, 2).Value = "Age";
            ws2.Cell(1, 3).Value = "Salary";
            ws2.Cell(2, 1).Value = "Veli";
            ws2.Cell(2, 2).Value = 25;
            ws2.Cell(2, 3).Value = 4000;
        });

        var records = await _reader.ReadAsync<ExcelTestRecord>(stream, new ExcelReadOptions { SheetName = "Second" });

        records.Should().ContainSingle();
        records[0].Name.Should().Be("Veli");
    }

    [Fact]
    public async Task ReadAsync_WithHeaderRow_SkipsRowsBeforeHeader()
    {
        using var stream = CreateExcelStream(wb =>
        {
            var ws = wb.Worksheets.Add("Sheet1");
            ws.Cell(1, 1).Value = "Rapor Başlığı";
            ws.Cell(3, 1).Value = "Name";
            ws.Cell(3, 2).Value = "Age";
            ws.Cell(3, 3).Value = "Salary";
            ws.Cell(4, 1).Value = "Ali";
            ws.Cell(4, 2).Value = 30;
            ws.Cell(4, 3).Value = 5000;
        });

        var records = await _reader.ReadAsync<ExcelTestRecord>(stream, new ExcelReadOptions { HeaderRow = 3 });

        records.Should().ContainSingle();
        records[0].Name.Should().Be("Ali");
    }

    [Fact]
    public async Task ReadAllSheetsAsync_ReadsEverySheet()
    {
        using var stream = CreateExcelStream(wb =>
        {
            var ws1 = wb.Worksheets.Add("Istanbul");
            ws1.Cell(1, 1).Value = "Name";
            ws1.Cell(1, 2).Value = "Age";
            ws1.Cell(1, 3).Value = "Salary";
            ws1.Cell(2, 1).Value = "Ali";
            ws1.Cell(2, 2).Value = 30;
            ws1.Cell(2, 3).Value = 5000;

            var ws2 = wb.Worksheets.Add("Ankara");
            ws2.Cell(1, 1).Value = "Name";
            ws2.Cell(1, 2).Value = "Age";
            ws2.Cell(1, 3).Value = "Salary";
            ws2.Cell(2, 1).Value = "Veli";
            ws2.Cell(2, 2).Value = 25;
            ws2.Cell(2, 3).Value = 4000;
        });

        var result = await _reader.ReadAllSheetsAsync<ExcelTestRecord>(stream);

        result.Should().HaveCount(2);
        result["Istanbul"].Should().ContainSingle().Which.Name.Should().Be("Ali");
        result["Ankara"].Should().ContainSingle().Which.Name.Should().Be("Veli");
    }

    [Fact]
    public async Task Roundtrip_WriteAndRead_PreservesData()
    {
        var original = new[]
        {
            new ExcelTestRecord { Name = "Ali", Age = 30, Salary = 5000m },
            new ExcelTestRecord { Name = "Veli", Age = 25, Salary = 4000m }
        };

        using var stream = new MemoryStream();
        var writer = new ExcelFileWriter();
        await writer.WriteAsync(original, stream);

        stream.Position = 0;
        var result = await _reader.ReadAsync<ExcelTestRecord>(stream);

        result.Should().BeEquivalentTo(original);
    }

    private static MemoryStream CreateExcelStream(Action<XLWorkbook> configure)
    {
        var stream = new MemoryStream();
        using (var workbook = new XLWorkbook())
        {
            configure(workbook);
            workbook.SaveAs(stream);
        }
        stream.Position = 0;
        return stream;
    }
}
