using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Localization;

public static class LocalizationMicrosoftExtensions
{
    public static IServiceCollection AddLocalization(
        this IServiceCollection services,
        IConfigurationSection configSection,
        Action<LocalizationOptions>? configure = null)
    {
        var options = new LocalizationOptions();
        configSection.Bind(options);
        configure?.Invoke(options);

        return AddLocalization(services, options);
    }

    public static IServiceCollection AddLocalization(
        this IServiceCollection services,
        LocalizationOptions options)
    {
        services.AddLocalization(opts => opts.ResourcesPath = options.ResourcesPath);
        services.AddSingleton(options);

        var supportedCultures = options.SupportedCultures.Length > 0
            ? options.SupportedCultures
            : [options.DefaultCulture];

        var cultures = supportedCultures
            .Select(c => new CultureInfo(c))
            .ToList();

        services.Configure<RequestLocalizationOptions>(opts =>
        {
            opts.DefaultRequestCulture = new RequestCulture(options.DefaultCulture);
            opts.SupportedCultures = cultures;
            opts.SupportedUICultures = cultures;
            opts.RequestCultureProviders = BuildProviders(options);
        });

        return services;
    }

    public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
    {
        app.UseRequestLocalization();
        return app;
    }

    internal static List<IRequestCultureProvider> BuildProviders(LocalizationOptions options)
    {
        var providers = new List<IRequestCultureProvider>();

        if (options.UseQueryString)
            providers.Add(new QueryStringRequestCultureProvider());

        if (options.UseCookie)
            providers.Add(new CookieRequestCultureProvider());

        if (options.UseHeader)
            providers.Add(new AcceptLanguageHeaderRequestCultureProvider());

        return providers;
    }
}
