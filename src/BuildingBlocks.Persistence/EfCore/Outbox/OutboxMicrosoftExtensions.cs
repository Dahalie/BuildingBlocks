using BuildingBlocks.Persistence.EfCore.DbContexts;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Persistence.EfCore.Outbox;

public static class OutboxMicrosoftExtensions
{
    public static IServiceCollection AddOutboxProcessor<TDbContext>(this IServiceCollection services, Action<OutboxProcessorOptions>? configure = null)
        where TDbContext : EfCoreDbContext
    {
        services.Configure<OutboxProcessorOptions>(opts =>
        {
            configure?.Invoke(opts);
        });

        services.AddHostedService<OutboxProcessor<TDbContext>>();

        return services;
    }

    public static IServiceCollection AddOutboxCleaner<TDbContext>(this IServiceCollection services, Action<OutboxCleanerOptions>? configure = null)
        where TDbContext : EfCoreDbContext
    {
        services.Configure<OutboxCleanerOptions>(opts =>
        {
            configure?.Invoke(opts);
        });

        services.AddHostedService<OutboxCleaner<TDbContext>>();

        return services;
    }
}
