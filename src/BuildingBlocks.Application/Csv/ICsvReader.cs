namespace BuildingBlocks.Application.Csv;

public interface ICsvReader
{
    Task<IReadOnlyList<T>> ReadAsync<T>(Stream stream, CancellationToken ct = default);
}
