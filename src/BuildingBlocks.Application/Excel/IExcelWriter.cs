namespace BuildingBlocks.Application.Excel;

public interface IExcelWriter
{
    Task WriteAsync<T>(IEnumerable<T> records, Stream destination, ExcelWriteOptions? options = null, CancellationToken ct = default);
    Task WriteSheetsAsync<T>(Dictionary<string, IEnumerable<T>> sheets, Stream destination, ExcelWriteOptions? options = null, CancellationToken ct = default);
}
