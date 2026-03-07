namespace BuildingBlocks.Application.Html;

public interface ITemplateRenderer
{
    Task<string> RenderAsync(string template, object model, CancellationToken ct = default);
}
