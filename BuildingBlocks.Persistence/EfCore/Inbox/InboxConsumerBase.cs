using System.Text.Json;
using BuildingBlocks.Application.Clock;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Persistence.EfCore.Inbox;

public abstract class InboxConsumerBase<TMessage>(
    EfCoreDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger logger)
    : IConsumer<TMessage>
    where TMessage : class, IIntegrationEvent
{
    public async Task Consume(ConsumeContext<TMessage> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;
        var consumerType = GetType().FullName!;

        var alreadyProcessed = await dbContext.Set<InboxMessage>()
            .AnyAsync(m => m.MessageId == message.MessageId && m.ConsumerType == consumerType, cancellationToken);

        if (alreadyProcessed)
        {
            logger.LogInformation(
                "Duplicate message {MessageId} skipped by {Consumer}",
                message.MessageId, GetType().Name);
            return;
        }

        var inboxMessage = new InboxMessage
        {
            Id           = Guid.CreateVersion7(),
            MessageId    = message.MessageId,
            EventType    = typeof(TMessage).FullName!,
            Payload      = JsonSerializer.Serialize(message, typeof(TMessage)),
            ReceivedOn   = dateTimeProvider.UtcNow,
            ConsumerType = GetType().FullName!
        };

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            dbContext.Set<InboxMessage>().Add(inboxMessage);
            await dbContext.SaveChangesAsync(cancellationToken);

            await HandleAsync(context, cancellationToken);

            inboxMessage.ProcessedOn = dateTimeProvider.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
        {
            logger.LogInformation(
                "Duplicate message {MessageId} skipped by {Consumer} (concurrent)",
                message.MessageId, GetType().Name);
        }
    }

    protected abstract Task HandleAsync(
        ConsumeContext<TMessage> context,
        CancellationToken cancellationToken);

    private static bool IsDuplicateKeyException(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true
        || ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) == true
        || ex.InnerException?.Message.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase) == true;
}
