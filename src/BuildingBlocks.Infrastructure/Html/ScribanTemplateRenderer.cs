using BuildingBlocks.Application.Html;
using Scriban;
using Scriban.Runtime;

namespace BuildingBlocks.Infrastructure.Html;

public class ScribanTemplateRenderer : ITemplateRenderer
{
    private static readonly MemberRenamerDelegate PreserveName = member => member.Name;

    public Task<string> RenderAsync(string template, object model, CancellationToken ct = default)
    {
        var parsedTemplate = Template.Parse(template);

        if (parsedTemplate.HasErrors)
            throw new InvalidOperationException(
                $"Template parse errors: {string.Join("; ", parsedTemplate.Messages)}");

        var scriptObject = new ScriptObject();
        scriptObject.Import(model, renamer: PreserveName);

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        context.MemberRenamer = PreserveName;

        var result = parsedTemplate.Render(context);
        return Task.FromResult(result);
    }
}
