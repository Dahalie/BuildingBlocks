using BuildingBlocks.Application.Clock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Clock;

public static class ClockMicrosoftExtensions
{
    public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
