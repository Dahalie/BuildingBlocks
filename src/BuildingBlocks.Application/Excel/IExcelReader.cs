namespace BuildingBlocks.Application.Excel;

public interface IExcelReader
{
    Task<IReadOnlyList<string>> GetSheetNamesAsync(Stream stream, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ReadAsync<T>(Stream stream, ExcelReadOptions? options = null, CancellationToken ct = default);
    Task<Dictionary<string, IReadOnlyList<T>>> ReadAllSheetsAsync<T>(Stream stream, ExcelReadOptions? options = null, CancellationToken ct = default);
}
