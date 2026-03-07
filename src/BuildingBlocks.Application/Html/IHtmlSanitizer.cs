namespace BuildingBlocks.Application.Html;

public interface IHtmlSanitizer
{
    string Sanitize(string html);
}
