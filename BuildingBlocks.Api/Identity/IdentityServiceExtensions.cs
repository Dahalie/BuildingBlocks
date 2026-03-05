using BuildingBlocks.Application.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Identity;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddCurrentUserProvider(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }

    public static IServiceCollection AddFakeCurrentUserProvider(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserProvider, FakeCurrentUserProvider>();

        return services;
    }
}