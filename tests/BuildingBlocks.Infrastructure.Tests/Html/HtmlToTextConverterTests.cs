using BuildingBlocks.Application.Html;
using BuildingBlocks.Infrastructure.Html;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Html;

public class HtmlToTextConverterTests
{
    private readonly HtmlToTextConverter _converter = new();

    [Fact]
    public void ImplementsIHtmlToTextConverter()
    {
        _converter.Should().BeAssignableTo<IHtmlToTextConverter>();
    }

    [Fact]
    public void Convert_StripsHtmlTags()
    {
        var html = "<h1>Başlık</h1><p>Paragraf</p>";

        var result = _converter.Convert(html);

        result.Should().Contain("Başlık");
        result.Should().Contain("Paragraf");
        result.Should().NotContain("<h1>");
        result.Should().NotContain("<p>");
    }

    [Fact]
    public void Convert_RemovesScriptContent()
    {
        var html = "<p>Görünür</p><script>var x = 1;</script>";

        var result = _converter.Convert(html);

        result.Should().Contain("Görünür");
        result.Should().NotContain("var x");
    }

    [Fact]
    public void Convert_RemovesStyleContent()
    {
        var html = "<style>body { color: red; }</style><p>İçerik</p>";

        var result = _converter.Convert(html);

        result.Should().Contain("İçerik");
        result.Should().NotContain("color");
    }

    [Fact]
    public void Convert_DecodesHtmlEntities()
    {
        var html = "<p>A &amp; B &lt; C</p>";

        var result = _converter.Convert(html);

        result.Should().Contain("A & B < C");
    }

    [Fact]
    public void Convert_TrimsEmptyLines()
    {
        var html = "<p>Satır 1</p>\n\n\n<p>Satır 2</p>";

        var result = _converter.Convert(html);

        result.Should().NotContain("\n\n");
    }

    [Fact]
    public void Convert_EmptyHtml_ReturnsEmpty()
    {
        var result = _converter.Convert("");

        result.Should().BeEmpty();
    }
}
