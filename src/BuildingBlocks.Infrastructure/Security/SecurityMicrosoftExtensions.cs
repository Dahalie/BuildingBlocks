using BuildingBlocks.Application.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Security;

public static class SecurityMicrosoftExtensions
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfigurationSection configSection,
        Action<AesEncryptionOptions>? configure = null)
    {
        var options = new AesEncryptionOptions { Key = null! };
        configSection.Bind(options);
        configure?.Invoke(options);

        return AddSecurity(services, options);
    }

    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        AesEncryptionOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<IHasher, Hasher>();
        services.AddSingleton<IEncryptor, AesEncryptor>();

        return services;
    }
}
