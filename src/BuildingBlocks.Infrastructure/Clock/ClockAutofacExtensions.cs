using Autofac;
using BuildingBlocks.Application.Clock;

namespace BuildingBlocks.Infrastructure.Clock;

public static class ClockAutofacExtensions
{
    public static ContainerBuilder AddDateTimeProvider(this ContainerBuilder builder)
    {
        builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance().IfNotRegistered(typeof(IDateTimeProvider));

        return builder;
    }
}