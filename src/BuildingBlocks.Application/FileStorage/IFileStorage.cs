namespace BuildingBlocks.Application.FileStorage;

public interface IFileStorage
{
    Task<string> SaveAsync(string relativePath, Stream content, CancellationToken ct = default);
    Task<Stream> ReadAsync(string relativePath, CancellationToken ct = default);
    Task DeleteAsync(string relativePath, CancellationToken ct = default);
    Task<bool> ExistsAsync(string relativePath, CancellationToken ct = default);
}
