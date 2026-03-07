using BuildingBlocks.Api.Localization;
using FluentAssertions;

namespace BuildingBlocks.Api.Tests.Localization;

public class LocalizationOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new LocalizationOptions();

        options.DefaultCulture.Should().Be("en");
        options.SupportedCultures.Should().BeEmpty();
        options.ResourcesPath.Should().Be("Resources");
        options.UseHeader.Should().BeTrue();
        options.UseQueryString.Should().BeFalse();
        options.UseCookie.Should().BeFalse();
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var options = new LocalizationOptions
        {
            DefaultCulture = "tr",
            SupportedCultures = ["tr", "en", "de"],
            ResourcesPath = "Lang",
            UseHeader = false,
            UseQueryString = true,
            UseCookie = true
        };

        options.DefaultCulture.Should().Be("tr");
        options.SupportedCultures.Should().HaveCount(3);
        options.ResourcesPath.Should().Be("Lang");
        options.UseHeader.Should().BeFalse();
        options.UseQueryString.Should().BeTrue();
        options.UseCookie.Should().BeTrue();
    }
}
