using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Messaging;

public static class MessagingMicrosoftExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, params Action<IBusRegistrationConfigurator>[] moduleConfigurations)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            foreach (var configure in moduleConfigurations)
                configure(config);

            config.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });

        return services;
    }
}