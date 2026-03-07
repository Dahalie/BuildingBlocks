using BuildingBlocks.Application.Html;
using BuildingBlocks.Infrastructure.Html;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Html;

public class ScribanTemplateRendererTests
{
    private readonly ScribanTemplateRenderer _renderer = new();

    [Fact]
    public void ImplementsITemplateRenderer()
    {
        _renderer.Should().BeAssignableTo<ITemplateRenderer>();
    }

    [Fact]
    public async Task RenderAsync_ReplacesVariables()
    {
        var template = "<h1>Merhaba {{Name}}</h1><p>Yaş: {{Age}}</p>";
        var model = new { Name = "Ali", Age = 30 };

        var result = await _renderer.RenderAsync(template, model);

        result.Should().Be("<h1>Merhaba Ali</h1><p>Yaş: 30</p>");
    }

    [Fact]
    public async Task RenderAsync_SupportsLoops()
    {
        var template = "{{ for item in Items }}{{item}},{{ end }}";
        var model = new { Items = new[] { "A", "B", "C" } };

        var result = await _renderer.RenderAsync(template, model);

        result.Should().Be("A,B,C,");
    }

    [Fact]
    public async Task RenderAsync_SupportsConditionals()
    {
        var template = "{{ if Active }}Aktif{{ else }}Pasif{{ end }}";

        var active = await _renderer.RenderAsync(template, new { Active = true });
        var passive = await _renderer.RenderAsync(template, new { Active = false });

        active.Should().Be("Aktif");
        passive.Should().Be("Pasif");
    }

    [Fact]
    public async Task RenderAsync_InvalidTemplate_ThrowsInvalidOperationException()
    {
        var template = "{{ if }}"; // geçersiz syntax

        var act = () => _renderer.RenderAsync(template, new { });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .Where(e => e.Message.Contains("Template parse errors"));
    }

    [Fact]
    public async Task RenderAsync_NoVariables_ReturnsTemplateAsIs()
    {
        var template = "<p>Sabit içerik</p>";

        var result = await _renderer.RenderAsync(template, new { });

        result.Should().Be("<p>Sabit içerik</p>");
    }
}
