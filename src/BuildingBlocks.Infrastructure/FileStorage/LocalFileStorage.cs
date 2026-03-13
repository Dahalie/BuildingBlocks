using BuildingBlocks.Application.FileStorage;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Infrastructure.FileStorage;

public class LocalFileStorage(IConfiguration configuration) : IFileStorage
{
    private readonly string _basePath = configuration.GetValue<string>("FileStorage:BasePath") ?? "./storage";

    public async Task<string> SaveAsync(string relativePath, Stream content, CancellationToken ct = default)
    {
        var fullPath = ResolveSafePath(relativePath);
        var directory = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(directory);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream, ct);

        return relativePath;
    }

    public async Task<Stream> ReadAsync(string relativePath, CancellationToken ct = default)
    {
        var fullPath = ResolveSafePath(relativePath);
        var memoryStream = new MemoryStream();
        await using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        await fileStream.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task DeleteAsync(string relativePath, CancellationToken ct = default)
    {
        var fullPath = ResolveSafePath(relativePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string relativePath, CancellationToken ct = default)
    {
        var fullPath = ResolveSafePath(relativePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    private string ResolveSafePath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_basePath, relativePath));
        var baseFull = Path.GetFullPath(_basePath);

        if (!fullPath.StartsWith(baseFull, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Path '{relativePath}' is outside the base storage directory.");

        return fullPath;
    }
}
