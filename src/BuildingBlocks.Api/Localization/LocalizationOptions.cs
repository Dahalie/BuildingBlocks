namespace BuildingBlocks.Api.Localization;

public class LocalizationOptions
{
    public string DefaultCulture { get; set; } = "en";
    public string[] SupportedCultures { get; set; } = [];
    public string ResourcesPath { get; set; } = "Resources";
    public bool UseHeader { get; set; } = true;
    public bool UseQueryString { get; set; }
    public bool UseCookie { get; set; }
}
