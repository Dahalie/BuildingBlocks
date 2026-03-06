using BuildingBlocks.Persistence.EfCore.DbContexts;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Persistence.EfCore.Inbox;

public static class InboxMicrosoftExtensions
{
    public static IServiceCollection AddInboxCleaner<TDbContext>(
        this IServiceCollection services,
        Action<InboxCleanerOptions>? configure = null)
        where TDbContext : EfCoreDbContext
    {
        services.Configure<InboxCleanerOptions>(opts =>
        {
            configure?.Invoke(opts);
        });

        services.AddHostedService<InboxCleaner<TDbContext>>();

        return services;
    }
}
