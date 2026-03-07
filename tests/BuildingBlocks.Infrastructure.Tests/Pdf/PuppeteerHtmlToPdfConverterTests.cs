using BuildingBlocks.Application.Pdf;
using BuildingBlocks.Infrastructure.Pdf;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Pdf;

public class PuppeteerHtmlToPdfConverterTests
{
    [Fact]
    public void ImplementsIHtmlToPdfConverter()
    {
        var converter = new PuppeteerHtmlToPdfConverter();

        converter.Should().BeAssignableTo<IHtmlToPdfConverter>();
    }

    [Fact]
    public void ImplementsIAsyncDisposable()
    {
        var converter = new PuppeteerHtmlToPdfConverter();

        converter.Should().BeAssignableTo<IAsyncDisposable>();
    }

    [Fact(Skip = "Requires Chromium download — run manually as integration test")]
    public async Task ConvertAsync_SimpleHtml_ProducesValidPdf()
    {
        await using var converter = new PuppeteerHtmlToPdfConverter();
        using var stream = new MemoryStream();

        await converter.ConvertAsync("<html><body><h1>Test</h1></body></html>", stream);

        stream.Length.Should().BeGreaterThan(0);
        stream.Position = 0;

        // PDF files start with %PDF
        using var reader = new StreamReader(stream);
        var header = await reader.ReadLineAsync();
        header.Should().StartWith("%PDF");
    }

    [Fact(Skip = "Requires Chromium download — run manually as integration test")]
    public async Task ConvertAsync_MultipleCalls_ReusesBrowser()
    {
        await using var converter = new PuppeteerHtmlToPdfConverter();

        using var stream1 = new MemoryStream();
        await converter.ConvertAsync("<h1>First</h1>", stream1);

        using var stream2 = new MemoryStream();
        await converter.ConvertAsync("<h1>Second</h1>", stream2);

        stream1.Length.Should().BeGreaterThan(0);
        stream2.Length.Should().BeGreaterThan(0);
    }
}
