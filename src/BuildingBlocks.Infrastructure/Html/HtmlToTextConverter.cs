using BuildingBlocks.Application.Html;
using HtmlAgilityPack;

namespace BuildingBlocks.Infrastructure.Html;

public class HtmlToTextConverter : IHtmlToTextConverter
{
    public string Convert(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        RemoveNodes(doc, "script");
        RemoveNodes(doc, "style");

        var text = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);

        var lines = text
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0);

        return string.Join(Environment.NewLine, lines);
    }

    private static void RemoveNodes(HtmlDocument doc, string tagName)
    {
        var nodes = doc.DocumentNode.SelectNodes($"//{tagName}");
        if (nodes is null) return;

        foreach (var node in nodes)
            node.Remove();
    }
}
