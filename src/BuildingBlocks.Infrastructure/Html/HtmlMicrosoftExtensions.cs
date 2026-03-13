using BuildingBlocks.Application.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Html;

public static class HtmlMicrosoftExtensions
{
    public static IServiceCollection AddHtml(this IServiceCollection services)
    {
        services.TryAddSingleton<ITemplateRenderer, ScribanTemplateRenderer>();
        services.TryAddSingleton<IHtmlSanitizer, GanssHtmlSanitizer>();
        services.TryAddSingleton<IHtmlToTextConverter, HtmlToTextConverter>();

        return services;
    }
}
