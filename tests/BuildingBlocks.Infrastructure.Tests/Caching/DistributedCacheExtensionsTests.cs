using BuildingBlocks.Infrastructure.Caching;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Infrastructure.Tests.Caching;

public class DistributedCacheExtensionsTests
{
    private readonly IDistributedCache _cache = new MemoryDistributedCache(
        Options.Create(new MemoryDistributedCacheOptions()));

    private sealed record TestItem(string Name, int Value);

    [Fact]
    public async Task SetAsync_GetAsync_RoundTrip()
    {
        var item = new TestItem("test", 42);

        await _cache.SetAsync("key1", item);
        var result = await _cache.GetAsync<TestItem>("key1");

        result.Should().NotBeNull();
        result!.Name.Should().Be("test");
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task GetAsync_NonExistentKey_ReturnsDefault()
    {
        var result = await _cache.GetAsync<TestItem>("missing");

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithOptions_Succeeds()
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetAsync("key2", new TestItem("cached", 1), options);
        var result = await _cache.GetAsync<TestItem>("key2");

        result.Should().NotBeNull();
        result!.Name.Should().Be("cached");
    }

    [Fact]
    public async Task GetOrSetAsync_CacheMiss_CallsFactory()
    {
        var factoryCalled = false;

        var result = await _cache.GetOrSetAsync("key3", async _ =>
        {
            factoryCalled = true;
            return new TestItem("created", 99);
        });

        factoryCalled.Should().BeTrue();
        result.Name.Should().Be("created");
        result.Value.Should().Be(99);
    }

    [Fact]
    public async Task GetOrSetAsync_CacheHit_DoesNotCallFactory()
    {
        await _cache.SetAsync("key4", new TestItem("existing", 1));
        var factoryCalled = false;

        var result = await _cache.GetOrSetAsync("key4", async _ =>
        {
            factoryCalled = true;
            return new TestItem("new", 2);
        });

        factoryCalled.Should().BeFalse();
        result.Name.Should().Be("existing");
    }

    [Fact]
    public async Task GetOrSetAsync_StoresValueInCache()
    {
        await _cache.GetOrSetAsync("key5", async _ => new TestItem("stored", 7));

        var cached = await _cache.GetAsync<TestItem>("key5");
        cached.Should().NotBeNull();
        cached!.Name.Should().Be("stored");
    }

    [Fact]
    public async Task SetAsync_OverwritesExistingValue()
    {
        await _cache.SetAsync("key6", new TestItem("old", 1));
        await _cache.SetAsync("key6", new TestItem("new", 2));

        var result = await _cache.GetAsync<TestItem>("key6");
        result!.Name.Should().Be("new");
        result.Value.Should().Be(2);
    }
}
