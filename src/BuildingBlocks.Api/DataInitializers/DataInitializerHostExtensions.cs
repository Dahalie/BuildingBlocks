using BuildingBlocks.Application.DataInitializers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Api.DataInitializers;

public static class DataInitializerHostExtensions
{
    public static async Task RunDataInitializersAsync(this IHost host, CancellationToken ct = default)
    {
        using var scope = host.Services.CreateScope();

        var initializers = scope.ServiceProvider
            .GetServices<IDataInitializer>()
            .OrderBy(x => x.Order);

        foreach (var initializer in initializers)
        {
            await initializer.InitializeAsync(ct);
        }
    }
}
