using System.Security.Cryptography;
using BuildingBlocks.Application.Security;
using BuildingBlocks.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Tests.Security;

public class SecurityMicrosoftExtensionsTests
{
    private static string GenerateBase64Key()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }

    [Fact]
    public void AddSecurity_WithOptions_RegistersServices()
    {
        var services = new ServiceCollection();
        var options = new AesEncryptionOptions { Key = GenerateBase64Key() };

        services.AddSecurity(options);

        var provider = services.BuildServiceProvider();

        provider.GetService<IHasher>().Should().NotBeNull();
        provider.GetService<IEncryptor>().Should().NotBeNull();
    }

    [Fact]
    public void AddSecurity_WithConfigSection_BindsOptions()
    {
        var services = new ServiceCollection();
        var key = GenerateBase64Key();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Security:Key"] = key
            })
            .Build();

        services.AddSecurity(config.GetSection("Security"));

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<AesEncryptionOptions>();

        resolved.Key.Should().Be(key);
    }

    [Fact]
    public void AddSecurity_WithConfigureCallback_OverridesConfig()
    {
        var services = new ServiceCollection();
        var overrideKey = GenerateBase64Key();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Security:Key"] = GenerateBase64Key()
            })
            .Build();

        services.AddSecurity(
            config.GetSection("Security"),
            opts => opts.Key = overrideKey);

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<AesEncryptionOptions>();

        resolved.Key.Should().Be(overrideKey);
    }

    [Fact]
    public void AddSecurity_ReturnsServiceCollection_ForChaining()
    {
        var services = new ServiceCollection();
        var options = new AesEncryptionOptions { Key = GenerateBase64Key() };

        var result = services.AddSecurity(options);

        result.Should().BeSameAs(services);
    }
}
