using BuildingBlocks.Persistence.EfCore.DbContexts;

namespace BuildingBlocks.Persistence.EfCore.Outbox;

internal interface IOutboxMessageStore<TDbContext>
    where TDbContext : EfCoreDbContext
{
    IReadOnlyList<OutboxMessage> GetStagedMessages();
    void Clear();
}
