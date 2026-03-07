using Autofac;
using BuildingBlocks.Application.Security;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Infrastructure.Security;

public static class SecurityAutofacExtensions
{
    public static ContainerBuilder AddSecurity(
        this ContainerBuilder builder,
        IConfigurationSection configSection)
    {
        var options = new AesEncryptionOptions { Key = null! };
        configSection.Bind(options);

        return AddSecurity(builder, options);
    }

    public static ContainerBuilder AddSecurity(
        this ContainerBuilder builder,
        AesEncryptionOptions options)
    {
        builder.RegisterInstance(options).AsSelf().SingleInstance();
        builder.RegisterType<Hasher>().As<IHasher>().SingleInstance();
        builder.RegisterType<AesEncryptor>().As<IEncryptor>().SingleInstance();

        return builder;
    }
}
