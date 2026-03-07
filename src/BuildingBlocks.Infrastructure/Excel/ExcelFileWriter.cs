using System.Reflection;
using BuildingBlocks.Application.Excel;
using ClosedXML.Excel;

namespace BuildingBlocks.Infrastructure.Excel;

public class ExcelFileWriter : IExcelWriter
{
    public Task WriteAsync<T>(IEnumerable<T> records, Stream destination, ExcelWriteOptions? options = null, CancellationToken ct = default)
    {
        options ??= new ExcelWriteOptions();
        using var workbook = new XLWorkbook();

        WriteSheet(workbook, options.SheetName, records);
        workbook.SaveAs(destination);

        return Task.CompletedTask;
    }

    public Task WriteSheetsAsync<T>(Dictionary<string, IEnumerable<T>> sheets, Stream destination, ExcelWriteOptions? options = null, CancellationToken ct = default)
    {
        using var workbook = new XLWorkbook();

        foreach (var (sheetName, records) in sheets)
            WriteSheet(workbook, sheetName, records);

        workbook.SaveAs(destination);

        return Task.CompletedTask;
    }

    private static void WriteSheet<T>(XLWorkbook workbook, string sheetName, IEnumerable<T> records)
    {
        var worksheet = workbook.Worksheets.Add(sheetName);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        for (var col = 0; col < properties.Length; col++)
            worksheet.Cell(1, col + 1).Value = properties[col].Name;

        var row = 2;
        foreach (var record in records)
        {
            for (var col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(record);
                var cell = worksheet.Cell(row, col + 1);
                SetCellValue(cell, value);
            }
            row++;
        }
    }

    private static void SetCellValue(IXLCell cell, object? value)
    {
        switch (value)
        {
            case null:
                cell.Value = Blank.Value;
                break;
            case string s:
                cell.Value = s;
                break;
            case int i:
                cell.Value = i;
                break;
            case long l:
                cell.Value = l;
                break;
            case double d:
                cell.Value = d;
                break;
            case decimal dec:
                cell.Value = (double)dec;
                break;
            case float f:
                cell.Value = f;
                break;
            case bool b:
                cell.Value = b;
                break;
            case DateTime dt:
                cell.Value = dt;
                break;
            case DateTimeOffset dto:
                cell.Value = dto.DateTime;
                break;
            default:
                cell.Value = value.ToString();
                break;
        }
    }
}
