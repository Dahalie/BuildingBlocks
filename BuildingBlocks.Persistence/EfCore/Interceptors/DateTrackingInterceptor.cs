using BuildingBlocks.Application.Clock;
using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.EfCore.Interceptors;

public class DateTrackingInterceptor(IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyDateTracking(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            ApplyDateTracking(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    private void ApplyDateTracking(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<IDateTrackable>().Where(e => e.State is EntityState.Added or EntityState.Modified);

        var now = dateTimeProvider.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
    }
}