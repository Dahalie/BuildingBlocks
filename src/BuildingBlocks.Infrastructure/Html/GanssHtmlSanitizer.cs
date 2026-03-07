namespace BuildingBlocks.Infrastructure.Html;

public class GanssHtmlSanitizer : Application.Html.IHtmlSanitizer
{
    private readonly Ganss.Xss.HtmlSanitizer _sanitizer = new();

    public string Sanitize(string html)
    {
        return _sanitizer.Sanitize(html);
    }
}
