using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BuildingBlocks.Infrastructure.Caching;

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetAsync<T>(
        this IDistributedCache cache,
        string key,
        CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(key, ct);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes);
    }

    public static async Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions? options = null,
        CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        await cache.SetAsync(key, bytes, options ?? new DistributedCacheEntryOptions(), ct);
    }

    public static async Task<T> GetOrSetAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<CancellationToken, Task<T>> factory,
        DistributedCacheEntryOptions? options = null,
        CancellationToken ct = default)
    {
        var existing = await cache.GetAsync<T>(key, ct);
        if (existing is not null)
            return existing;

        var value = await factory(ct);
        await cache.SetAsync(key, value, options, ct);
        return value;
    }
}
