using BuildingBlocks.Application.Identity;
using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.EfCore.Interceptors;

public class AuditingInterceptor<TUserId>(ICurrentUserProvider<TUserId> currentUserProvider) : SaveChangesInterceptor
    where TUserId : struct
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            ApplyAuditing(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyAuditing(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditing(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<IAuditable<TUserId>>().Where(e => e.State is EntityState.Added or EntityState.Modified);

        var userId = currentUserProvider.UserId;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && EqualityComparer<TUserId>.Default.Equals(entry.Entity.CreatedBy, default))
                entry.Entity.CreatedBy = userId;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedBy = userId;
        }
    }
}