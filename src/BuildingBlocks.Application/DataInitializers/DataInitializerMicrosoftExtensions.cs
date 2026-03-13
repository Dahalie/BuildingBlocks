using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Application.DataInitializers;

public static class DataInitializerMicrosoftExtensions
{
    public static IServiceCollection AddDataInitializersFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        var descriptors = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IDataInitializer).IsAssignableFrom(t))
            .Select(t => ServiceDescriptor.Scoped(typeof(IDataInitializer), t));

        services.TryAddEnumerable(descriptors);

        return services;
    }
}
