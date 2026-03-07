using BuildingBlocks.Application.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Email;

public static class SmtpEmailMicrosoftExtensions
{
    public static IServiceCollection AddSmtpEmail(
        this IServiceCollection services,
        IConfigurationSection configSection,
        Action<SmtpOptions>? configure = null)
    {
        var options = new SmtpOptions
        {
            Host = null!,
            SenderAddress = null!
        };
        configSection.Bind(options);
        configure?.Invoke(options);

        return AddSmtpEmail(services, options);
    }

    public static IServiceCollection AddSmtpEmail(
        this IServiceCollection services,
        SmtpOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
