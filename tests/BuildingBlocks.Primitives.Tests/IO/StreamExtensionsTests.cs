using System.Text;
using BuildingBlocks.Primitives.IO;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.IO;

public class StreamExtensionsTests
{
    [Fact]
    public async Task ToByteArrayAsync_ReturnsCorrectBytes()
    {
        var data = Encoding.UTF8.GetBytes("hello world");
        using var stream = new MemoryStream(data);

        var result = await stream.ToByteArrayAsync();

        result.Should().Equal(data);
    }

    [Fact]
    public async Task ToByteArrayAsync_EmptyStream_ReturnsEmptyArray()
    {
        using var stream = new MemoryStream();

        var result = await stream.ToByteArrayAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ToByteArrayAsync_MemoryStream_UsesToArray()
    {
        using var stream = new MemoryStream([1, 2, 3]);

        var result = await stream.ToByteArrayAsync();

        result.Should().Equal([1, 2, 3]);
    }

    [Fact]
    public async Task ToStringAsync_DefaultEncoding_ReturnsUtf8()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("merhaba"));

        var result = await stream.ToStringAsync();

        result.Should().Be("merhaba");
    }

    [Fact]
    public async Task ToStringAsync_WithEncoding_UsesSpecifiedEncoding()
    {
        var text = "test data";
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

        var result = await stream.ToStringAsync(Encoding.ASCII);

        result.Should().Be(text);
    }

    [Fact]
    public async Task ToStringAsync_SeekableStream_ResetsPosition()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("hello"));
        stream.Position = 3; // move past beginning

        var result = await stream.ToStringAsync();

        result.Should().Be("hello");
    }
}
