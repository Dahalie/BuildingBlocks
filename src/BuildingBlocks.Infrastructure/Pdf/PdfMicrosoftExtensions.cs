using BuildingBlocks.Application.Pdf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Pdf;

public static class PdfMicrosoftExtensions
{
    public static IServiceCollection AddHtmlToPdfConverter(this IServiceCollection services)
    {
        services.TryAddSingleton<IHtmlToPdfConverter, PuppeteerHtmlToPdfConverter>();

        return services;
    }
}
