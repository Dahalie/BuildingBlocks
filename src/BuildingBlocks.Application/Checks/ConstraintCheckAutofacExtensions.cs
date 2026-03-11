using System.Reflection;
using Autofac;
using BuildingBlocks.Domain.Checks;

namespace BuildingBlocks.Application.Checks;

[Obsolete("Use PolicyAutofacExtensions.AddPoliciesFromAssemblies from BuildingBlocks.Application.Policies instead.")]
public static class ConstraintCheckAutofacExtensions
{
    public static ContainerBuilder AddConstraintChecksFromAssemblies(this ContainerBuilder builder, params Assembly[] assemblies)
    {
        builder.RegisterAssemblyTypes(assemblies).Where(type => !type.IsAbstract && type.IsClosedTypeOf(typeof(IConstraintCheck<,>))).AsImplementedInterfaces().InstancePerLifetimeScope();

        return builder;
    }
}
