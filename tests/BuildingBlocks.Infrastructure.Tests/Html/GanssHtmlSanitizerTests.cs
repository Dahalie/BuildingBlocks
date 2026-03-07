using BuildingBlocks.Application.Html;
using BuildingBlocks.Infrastructure.Html;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Html;

public class GanssHtmlSanitizerTests
{
    private readonly GanssHtmlSanitizer _sanitizer = new();

    [Fact]
    public void ImplementsIHtmlSanitizer()
    {
        _sanitizer.Should().BeAssignableTo<IHtmlSanitizer>();
    }

    [Fact]
    public void Sanitize_RemovesScriptTags()
    {
        var html = "<p>Merhaba</p><script>alert('xss')</script>";

        var result = _sanitizer.Sanitize(html);

        result.Should().NotContain("<script>");
        result.Should().Contain("<p>Merhaba</p>");
    }

    [Fact]
    public void Sanitize_RemovesOnClickAttributes()
    {
        var html = "<a href=\"#\" onclick=\"alert('xss')\">Link</a>";

        var result = _sanitizer.Sanitize(html);

        result.Should().NotContain("onclick");
        result.Should().Contain("Link");
    }

    [Fact]
    public void Sanitize_RemovesIframeTags()
    {
        var html = "<p>İçerik</p><iframe src=\"evil.com\"></iframe>";

        var result = _sanitizer.Sanitize(html);

        result.Should().NotContain("<iframe");
        result.Should().Contain("<p>İçerik</p>");
    }

    [Fact]
    public void Sanitize_KeepsSafeTags()
    {
        var html = "<h1>Başlık</h1><p>Paragraf</p><strong>Kalın</strong><em>İtalik</em>";

        var result = _sanitizer.Sanitize(html);

        result.Should().Contain("<h1>Başlık</h1>");
        result.Should().Contain("<p>Paragraf</p>");
        result.Should().Contain("<strong>Kalın</strong>");
        result.Should().Contain("<em>İtalik</em>");
    }

    [Fact]
    public void Sanitize_EmptyString_ReturnsEmpty()
    {
        var result = _sanitizer.Sanitize("");

        result.Should().BeEmpty();
    }
}
