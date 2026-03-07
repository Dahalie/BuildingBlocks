using Autofac;
using BuildingBlocks.Application.Email;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Infrastructure.Email;

public static class SmtpEmailAutofacExtensions
{
    public static ContainerBuilder AddSmtpEmail(
        this ContainerBuilder builder,
        IConfigurationSection configSection)
    {
        var options = new SmtpOptions
        {
            Host = null!,
            SenderAddress = null!
        };
        configSection.Bind(options);

        return AddSmtpEmail(builder, options);
    }

    public static ContainerBuilder AddSmtpEmail(
        this ContainerBuilder builder,
        SmtpOptions options)
    {
        builder.RegisterInstance(options).AsSelf().SingleInstance();
        builder.RegisterType<SmtpEmailSender>().As<IEmailSender>().SingleInstance();

        return builder;
    }
}
