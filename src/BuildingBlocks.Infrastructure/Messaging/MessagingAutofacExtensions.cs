using Autofac;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Infrastructure.Messaging;

public static class MessagingAutofacExtensions
{
    public static ContainerBuilder AddMassTransitMessageBus(this ContainerBuilder builder)
    {
        builder.RegisterType<MessageBus>().As<IMessageBus>().InstancePerLifetimeScope();

        return builder;
    }
}