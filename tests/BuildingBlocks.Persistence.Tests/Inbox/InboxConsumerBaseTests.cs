using BuildingBlocks.Application.Clock;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Contracts.Messaging;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using BuildingBlocks.Persistence.EfCore.Inbox;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;

namespace BuildingBlocks.Persistence.Tests.Inbox;

// --- Test infrastructure (shared with InboxCleanerTests) ---

public record TestIntegrationEvent(Guid TestId, string Data) : IntegrationEventBase;

public class TestInboxDbContext(DbContextOptions<TestInboxDbContext> options) : EfCoreDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyInboxConfiguration();
    }
}

// --- Tests ---

public class EfCoreInboxStoreTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly DateTimeOffset _fixedNow = new(2026, 3, 2, 12, 0, 0, TimeSpan.Zero);
    private const string HandlerType = "TestApp.HandleOrderCreatedCommand";

    private static TestInboxDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestInboxDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new TestInboxDbContext(options);
    }

    private EfCoreInboxStore<TestInboxDbContext> CreateStore(TestInboxDbContext dbContext)
    {
        _dateTimeProvider.UtcNow.Returns(_fixedNow);
        return new EfCoreInboxStore<TestInboxDbContext>(dbContext, _dateTimeProvider);
    }

    [Fact]
    public async Task ExistsAsync_NoRecord_ReturnsFalse()
    {
        using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);

        var exists = await store.ExistsAsync(Guid.NewGuid(), HandlerType);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task RecordAsync_NewMessage_InsertsInboxRecord()
    {
        using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var messageId = Guid.NewGuid();
        var messageType = typeof(TestIntegrationEvent).FullName!;

        await store.RecordAsync(messageId, messageType, HandlerType);

        var records = await dbContext.Set<InboxMessage>().ToListAsync();
        records.Should().HaveCount(1);

        var record = records[0];
        record.MessageId.Should().Be(messageId);
        record.EventType.Should().Be(messageType);
        record.ConsumerType.Should().Be(HandlerType);
        record.ReceivedOn.Should().Be(_fixedNow);
        record.ProcessedOn.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_AfterRecord_ReturnsTrue()
    {
        using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var messageId = Guid.NewGuid();

        await store.RecordAsync(messageId, "TestEvent", HandlerType);

        var exists = await store.ExistsAsync(messageId, HandlerType);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_DifferentHandlerType_ReturnsFalse()
    {
        using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var messageId = Guid.NewGuid();

        await store.RecordAsync(messageId, "TestEvent", HandlerType);

        var exists = await store.ExistsAsync(messageId, "DifferentHandler");
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task MarkProcessedAsync_SetsProcessedOn()
    {
        using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var messageId = Guid.NewGuid();

        await store.RecordAsync(messageId, "TestEvent", HandlerType);
        await store.MarkProcessedAsync(messageId, HandlerType);

        var record = await dbContext.Set<InboxMessage>().SingleAsync();
        record.ProcessedOn.Should().Be(_fixedNow);
    }
}
