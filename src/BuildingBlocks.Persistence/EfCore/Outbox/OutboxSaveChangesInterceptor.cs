using BuildingBlocks.Persistence.EfCore.DbContexts;
using BuildingBlocks.Persistence.EfCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.EfCore.Outbox;

internal class OutboxSaveChangesInterceptor<TDbContext>(IOutboxMessageStore<TDbContext> outboxMessageStore) : SaveChangesInterceptor, IOrderedInterceptor
    where TDbContext : EfCoreDbContext
{
    public int Order => 100;
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is TDbContext)
            FlushStagedMessages(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is TDbContext)
            FlushStagedMessages(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is TDbContext)
            outboxMessageStore.Clear();

        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is TDbContext)
            outboxMessageStore.Clear();

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (eventData.Context is TDbContext)
            DetachStagedMessages(eventData.Context);

        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is TDbContext)
            DetachStagedMessages(eventData.Context);

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void FlushStagedMessages(DbContext context)
    {
        var messages = outboxMessageStore.GetStagedMessages();

        if (messages.Count == 0)
            return;

        context.Set<OutboxMessage>().AddRange(messages);
    }

    private static void DetachStagedMessages(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<OutboxMessage>()
                     .Where(e => e.State == EntityState.Added)
                     .ToList())
        {
            entry.State = EntityState.Detached;
        }
    }
}
