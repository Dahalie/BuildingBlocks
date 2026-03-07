using BuildingBlocks.Api.Localization;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Api.Tests.Localization;

public class LocalizationMicrosoftExtensionsTests
{
    [Fact]
    public void AddLocalization_WithOptions_RegistersServices()
    {
        var services = new ServiceCollection();

        var options = new LocalizationOptions
        {
            DefaultCulture = "tr",
            SupportedCultures = ["tr", "en"]
        };

        services.AddLocalization(options);

        var provider = services.BuildServiceProvider();
        var requestOptions = provider.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

        requestOptions.DefaultRequestCulture.Culture.Name.Should().Be("tr");
        requestOptions.SupportedCultures.Should().HaveCount(2);
        requestOptions.SupportedUICultures.Should().HaveCount(2);
    }

    [Fact]
    public void AddLocalization_WithConfigSection_BindsOptions()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Localization:DefaultCulture"] = "de",
                ["Localization:SupportedCultures:0"] = "de",
                ["Localization:SupportedCultures:1"] = "en",
                ["Localization:ResourcesPath"] = "Lang"
            })
            .Build();

        services.AddLocalization(config.GetSection("Localization"));

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<LocalizationOptions>();

        resolved.DefaultCulture.Should().Be("de");
        resolved.SupportedCultures.Should().BeEquivalentTo(["de", "en"]);
        resolved.ResourcesPath.Should().Be("Lang");
    }

    [Fact]
    public void AddLocalization_WithConfigureCallback_OverridesConfig()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Localization:DefaultCulture"] = "en"
            })
            .Build();

        services.AddLocalization(
            config.GetSection("Localization"),
            opts => opts.DefaultCulture = "fr");

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<LocalizationOptions>();

        resolved.DefaultCulture.Should().Be("fr");
    }

    [Fact]
    public void BuildProviders_AllEnabled_ReturnsThreeProviders()
    {
        var options = new LocalizationOptions
        {
            UseHeader = true,
            UseQueryString = true,
            UseCookie = true
        };

        var providers = LocalizationMicrosoftExtensions.BuildProviders(options);

        providers.Should().HaveCount(3);
        providers.Should().ContainSingle(p => p is QueryStringRequestCultureProvider);
        providers.Should().ContainSingle(p => p is CookieRequestCultureProvider);
        providers.Should().ContainSingle(p => p is AcceptLanguageHeaderRequestCultureProvider);
    }

    [Fact]
    public void BuildProviders_OnlyHeader_ReturnsSingleProvider()
    {
        var options = new LocalizationOptions
        {
            UseHeader = true,
            UseQueryString = false,
            UseCookie = false
        };

        var providers = LocalizationMicrosoftExtensions.BuildProviders(options);

        providers.Should().HaveCount(1);
        providers[0].Should().BeOfType<AcceptLanguageHeaderRequestCultureProvider>();
    }

    [Fact]
    public void BuildProviders_NoneEnabled_ReturnsEmpty()
    {
        var options = new LocalizationOptions
        {
            UseHeader = false,
            UseQueryString = false,
            UseCookie = false
        };

        var providers = LocalizationMicrosoftExtensions.BuildProviders(options);

        providers.Should().BeEmpty();
    }

    [Fact]
    public void AddLocalization_ReturnsServiceCollection_ForChaining()
    {
        var services = new ServiceCollection();
        var options = new LocalizationOptions();

        var result = services.AddLocalization(options);

        result.Should().BeSameAs(services);
    }
}
