using System.Reflection;
using BuildingBlocks.Application.Excel;
using ClosedXML.Excel;

namespace BuildingBlocks.Infrastructure.Excel;

public class ExcelFileReader : IExcelReader
{
    public Task<IReadOnlyList<string>> GetSheetNamesAsync(Stream stream, CancellationToken ct = default)
    {
        using var workbook = new XLWorkbook(stream);

        IReadOnlyList<string> names = workbook.Worksheets
            .Select(ws => ws.Name)
            .ToList();

        return Task.FromResult(names);
    }

    public Task<IReadOnlyList<T>> ReadAsync<T>(Stream stream, ExcelReadOptions? options = null, CancellationToken ct = default)
    {
        options ??= new ExcelReadOptions();
        using var workbook = new XLWorkbook(stream);

        var worksheet = options.SheetName is not null
            ? workbook.Worksheet(options.SheetName)
            : workbook.Worksheets.First();

        var records = ReadSheet<T>(worksheet, options.HeaderRow);
        return Task.FromResult(records);
    }

    public Task<Dictionary<string, IReadOnlyList<T>>> ReadAllSheetsAsync<T>(Stream stream, ExcelReadOptions? options = null, CancellationToken ct = default)
    {
        options ??= new ExcelReadOptions();
        using var workbook = new XLWorkbook(stream);

        var result = new Dictionary<string, IReadOnlyList<T>>();

        foreach (var worksheet in workbook.Worksheets)
            result[worksheet.Name] = ReadSheet<T>(worksheet, options.HeaderRow);

        return Task.FromResult(result);
    }

    private static IReadOnlyList<T> ReadSheet<T>(IXLWorksheet worksheet, int headerRow)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToArray();

        var headerRowData = worksheet.Row(headerRow);
        var lastColumn = headerRowData.LastCellUsed()?.Address.ColumnNumber ?? 0;

        if (lastColumn == 0)
            return [];

        var columnMap = new Dictionary<int, PropertyInfo>();
        for (var col = 1; col <= lastColumn; col++)
        {
            var headerValue = headerRowData.Cell(col).GetString().Trim();
            var property = properties.FirstOrDefault(p =>
                string.Equals(p.Name, headerValue, StringComparison.OrdinalIgnoreCase));

            if (property is not null)
                columnMap[col] = property;
        }

        var records = new List<T>();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? headerRow;

        for (var row = headerRow + 1; row <= lastRow; row++)
        {
            var currentRow = worksheet.Row(row);
            if (currentRow.IsEmpty())
                continue;

            var record = Activator.CreateInstance<T>();

            foreach (var (col, property) in columnMap)
            {
                var cell = currentRow.Cell(col);
                var value = ConvertCellValue(cell, property.PropertyType);
                property.SetValue(record, value);
            }

            records.Add(record);
        }

        return records;
    }

    private static object? ConvertCellValue(IXLCell cell, Type targetType)
    {
        if (cell.IsEmpty())
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        return underlying switch
        {
            _ when underlying == typeof(string) => cell.GetString(),
            _ when underlying == typeof(int) => (int)cell.GetDouble(),
            _ when underlying == typeof(long) => (long)cell.GetDouble(),
            _ when underlying == typeof(double) => cell.GetDouble(),
            _ when underlying == typeof(decimal) => (decimal)cell.GetDouble(),
            _ when underlying == typeof(float) => (float)cell.GetDouble(),
            _ when underlying == typeof(bool) => cell.GetBoolean(),
            _ when underlying == typeof(DateTime) => cell.GetDateTime(),
            _ when underlying == typeof(DateTimeOffset) => new DateTimeOffset(cell.GetDateTime()),
            _ => Convert.ChangeType(cell.GetString(), underlying),
        };
    }
}
