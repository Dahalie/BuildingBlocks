namespace BuildingBlocks.Application.Csv;

public interface ICsvWriter
{
    Task WriteAsync<T>(IEnumerable<T> records, Stream destination, CancellationToken ct = default);
}
