using System.Reflection;
using Autofac;

namespace BuildingBlocks.Application.DomainServices;

public static class DomainServiceAutofacExtensions
{
    public static ContainerBuilder AddDomainServicesFromAssemblies(this ContainerBuilder builder, params Assembly[] assemblies)
    {
        builder.RegisterAssemblyTypes(assemblies).Where(type => type.IsAssignableTo(typeof(IDomainService))).AsImplementedInterfaces().InstancePerLifetimeScope();

        return builder;
    }
}