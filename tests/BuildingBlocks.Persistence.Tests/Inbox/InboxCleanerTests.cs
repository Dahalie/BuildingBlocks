using BuildingBlocks.Persistence.EfCore.Inbox;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.Tests.Inbox;

public class InboxCleanerTests
{
    private readonly DateTimeOffset _fixedNow = new(2026, 3, 2, 12, 0, 0, TimeSpan.Zero);

    private TestInboxDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestInboxDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new TestInboxDbContext(options);
    }

    private static InboxMessage CreateInboxMessage(
        DateTimeOffset receivedOn,
        DateTimeOffset? processedOn = null)
    {
        return new InboxMessage
        {
            Id           = Guid.CreateVersion7(),
            MessageId    = Guid.NewGuid(),
            EventType    = "TestEvent",
            Payload      = "{}",
            ReceivedOn   = receivedOn,
            ProcessedOn  = processedOn,
            ConsumerType = "TestConsumer"
        };
    }

    [Fact]
    public async Task CleaningLogic_DeletesProcessedMessagesOlderThanRetention()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var opts = new InboxCleanerOptions { RetentionDays = 7, BatchSize = 1000 };
        var cutoff = _fixedNow.AddDays(-opts.RetentionDays);

        var oldProcessed = CreateInboxMessage(
            receivedOn: _fixedNow.AddDays(-10),
            processedOn: _fixedNow.AddDays(-10));
        var recentProcessed = CreateInboxMessage(
            receivedOn: _fixedNow.AddDays(-1),
            processedOn: _fixedNow.AddDays(-1));
        var unprocessed = CreateInboxMessage(
            receivedOn: _fixedNow.AddDays(-10));

        dbContext.InboxMessages.AddRange(oldProcessed, recentProcessed, unprocessed);
        await dbContext.SaveChangesAsync();

        // Act — simulate cleaner query logic
        var toDelete = await dbContext.InboxMessages
            .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoff)
            .Take(opts.BatchSize)
            .ToListAsync();

        dbContext.InboxMessages.RemoveRange(toDelete);
        await dbContext.SaveChangesAsync();

        // Assert
        toDelete.Should().HaveCount(1);

        var remaining = await dbContext.InboxMessages.ToListAsync();
        remaining.Should().HaveCount(2);
        remaining.Should().Contain(m => m.Id == recentProcessed.Id);
        remaining.Should().Contain(m => m.Id == unprocessed.Id);
    }

    [Fact]
    public async Task CleaningLogic_KeepsUnprocessedMessages()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var opts = new InboxCleanerOptions { RetentionDays = 7, BatchSize = 1000 };
        var cutoff = _fixedNow.AddDays(-opts.RetentionDays);

        var unprocessed = CreateInboxMessage(
            receivedOn: _fixedNow.AddDays(-30));

        dbContext.InboxMessages.Add(unprocessed);
        await dbContext.SaveChangesAsync();

        // Act
        var toDelete = await dbContext.InboxMessages
            .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoff)
            .Take(opts.BatchSize)
            .ToListAsync();

        dbContext.InboxMessages.RemoveRange(toDelete);
        await dbContext.SaveChangesAsync();

        // Assert
        toDelete.Should().BeEmpty();
        var remaining = await dbContext.InboxMessages.ToListAsync();
        remaining.Should().HaveCount(1);
    }
}
