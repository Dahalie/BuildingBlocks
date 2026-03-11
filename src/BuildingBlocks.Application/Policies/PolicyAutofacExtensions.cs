using System.Reflection;
using Autofac;

namespace BuildingBlocks.Application.Policies;

public static class PolicyAutofacExtensions
{
    public static ContainerBuilder AddPoliciesFromAssemblies(this ContainerBuilder builder, params Assembly[] assemblies)
    {
        builder.RegisterAssemblyTypes(assemblies)
            .Where(type => !type.IsAbstract && typeof(IPolicy).IsAssignableFrom(type))
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        return builder;
    }
}
