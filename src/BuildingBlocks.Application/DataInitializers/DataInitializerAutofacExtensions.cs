using System.Reflection;
using Autofac;

namespace BuildingBlocks.Application.DataInitializers;

public static class DataInitializerAutofacExtensions
{
    public static ContainerBuilder AddDataInitializersFromAssemblies(this ContainerBuilder builder, params Assembly[] assemblies)
    {
        builder.RegisterAssemblyTypes(assemblies)
            .Where(type => !type.IsAbstract && typeof(IDataInitializer).IsAssignableFrom(type))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        return builder;
    }
}
