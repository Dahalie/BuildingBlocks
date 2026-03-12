using BuildingBlocks.Application.Clock;
using BuildingBlocks.Application.Messaging;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.Inbox;

public class EfCoreInboxStore<TDbContext>(TDbContext dbContext, IDateTimeProvider dateTimeProvider) : IInboxStore
    where TDbContext : DbContext
{
    public async Task<bool> ExistsAsync(Guid messageId, string handlerType, CancellationToken ct = default)
        => await dbContext.Set<InboxMessage>()
            .AnyAsync(m => m.MessageId == messageId && m.ConsumerType == handlerType, ct);

    public async Task RecordAsync(Guid messageId, string messageType, string handlerType, CancellationToken ct = default)
    {
        var transaction = dbContext.Database.CurrentTransaction;

        if (transaction is not null)
            await transaction.CreateSavepointAsync("inbox_record", ct);

        try
        {
            dbContext.Set<InboxMessage>().Add(new InboxMessage
            {
                Id           = Guid.CreateVersion7(),
                MessageId    = messageId,
                EventType    = messageType,
                Payload      = string.Empty,
                ReceivedOn   = dateTimeProvider.UtcNow,
                ConsumerType = handlerType
            });

            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
        {
            if (transaction is not null)
                await transaction.RollbackToSavepointAsync("inbox_record", ct);

            throw new InboxDuplicateMessageException(messageId, handlerType);
        }
    }

    public async Task MarkProcessedAsync(Guid messageId, string handlerType, CancellationToken ct = default)
    {
        var record = await dbContext.Set<InboxMessage>()
            .FirstOrDefaultAsync(m => m.MessageId == messageId && m.ConsumerType == handlerType, ct);

        if (record is not null)
        {
            record.ProcessedOn = dateTimeProvider.UtcNow;
            await dbContext.SaveChangesAsync(ct);
        }
    }

    private static bool IsDuplicateKeyException(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true
        || ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) == true
        || ex.InnerException?.Message.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase) == true;
}
