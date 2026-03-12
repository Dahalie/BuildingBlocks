using BuildingBlocks.Application.Clock;
using BuildingBlocks.Contracts.Messaging;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using BuildingBlocks.Persistence.EfCore.Inbox;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BuildingBlocks.Persistence.Tests.Inbox;

// --- Test infrastructure (namespace-level for NSubstitute proxy compatibility) ---

public record TestIntegrationEvent(Guid TestId, string Data) : IntegrationEventBase;

public class TestInboxDbContext(DbContextOptions<TestInboxDbContext> options) : EfCoreDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyInboxConfiguration();
    }
}

public class TestConsumer(
    TestInboxDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger logger)
    : InboxConsumerBase<TestInboxDbContext, TestIntegrationEvent>(dbContext, dateTimeProvider, logger)
{
    public bool HandleAsyncCalled { get; private set; }
    public ConsumeContext<TestIntegrationEvent>? CapturedContext { get; private set; }

    protected override Task HandleAsync(ConsumeContext<TestIntegrationEvent> context, CancellationToken cancellationToken)
    {
        HandleAsyncCalled = true;
        CapturedContext = context;
        return Task.CompletedTask;
    }
}

public class FailingConsumer(
    TestInboxDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger logger)
    : InboxConsumerBase<TestInboxDbContext, TestIntegrationEvent>(dbContext, dateTimeProvider, logger)
{
    protected override Task HandleAsync(ConsumeContext<TestIntegrationEvent> context, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Business logic failed");
    }
}

// --- Tests ---

public class InboxConsumerBaseTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly ILogger<TestConsumer> _logger = Substitute.For<ILogger<TestConsumer>>();
    private readonly DateTimeOffset _fixedNow = new(2026, 3, 2, 12, 0, 0, TimeSpan.Zero);

    private static TestInboxDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestInboxDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new TestInboxDbContext(options);
    }

    private static ConsumeContext<TestIntegrationEvent> CreateConsumeContext(TestIntegrationEvent message)
    {
        var context = Substitute.For<ConsumeContext<TestIntegrationEvent>>();
        context.Message.Returns(message);
        context.CancellationToken.Returns(CancellationToken.None);
        return context;
    }

    [Fact]
    public async Task Consume_NewMessage_InsertsInboxRecordAndCallsHandleAsync()
    {
        // Arrange
        _dateTimeProvider.UtcNow.Returns(_fixedNow);
        using var dbContext = CreateDbContext();
        var consumer = new TestConsumer(dbContext, _dateTimeProvider, _logger);

        var integrationEvent = new TestIntegrationEvent(Guid.NewGuid(), "test-data")
        {
            MessageId = Guid.NewGuid(),
            OccurredOn = _fixedNow.AddMinutes(-5)
        };
        var consumeContext = CreateConsumeContext(integrationEvent);

        // Act
        await consumer.Consume(consumeContext);

        // Assert
        consumer.HandleAsyncCalled.Should().BeTrue();
        consumer.CapturedContext.Should().Be(consumeContext);

        var inboxMessages = await dbContext.Set<InboxMessage>().ToListAsync();
        inboxMessages.Should().HaveCount(1);

        var inbox = inboxMessages[0];
        inbox.MessageId.Should().Be(integrationEvent.MessageId);
        inbox.ProcessedOn.Should().Be(_fixedNow);
        inbox.ConsumerType.Should().Contain("TestConsumer");
        inbox.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task Consume_DuplicateMessage_SkipsWithoutCallingHandleAsync()
    {
        // Arrange
        _dateTimeProvider.UtcNow.Returns(_fixedNow);
        using var dbContext = CreateDbContext();

        var messageId = Guid.NewGuid();
        var firstEvent = new TestIntegrationEvent(Guid.NewGuid(), "first")
        {
            MessageId = messageId,
            OccurredOn = _fixedNow.AddMinutes(-5)
        };
        var secondEvent = new TestIntegrationEvent(Guid.NewGuid(), "second")
        {
            MessageId = messageId,
            OccurredOn = _fixedNow.AddMinutes(-3)
        };

        // First consume succeeds
        var consumer1 = new TestConsumer(dbContext, _dateTimeProvider, _logger);
        await consumer1.Consume(CreateConsumeContext(firstEvent));

        // Second consume with same MessageId should skip
        var consumer2 = new TestConsumer(dbContext, _dateTimeProvider, _logger);
        await consumer2.Consume(CreateConsumeContext(secondEvent));

        // Assert
        consumer2.HandleAsyncCalled.Should().BeFalse();

        var inboxMessages = await dbContext.Set<InboxMessage>().ToListAsync();
        inboxMessages.Should().HaveCount(1);
    }

    [Fact]
    public async Task Consume_HandlerThrows_RethrowsException()
    {
        // Arrange
        _dateTimeProvider.UtcNow.Returns(_fixedNow);
        using var dbContext = CreateDbContext();
        var failingLogger = Substitute.For<ILogger<FailingConsumer>>();
        var consumer = new FailingConsumer(dbContext, _dateTimeProvider, failingLogger);

        var integrationEvent = new TestIntegrationEvent(Guid.NewGuid(), "fail-data")
        {
            MessageId = Guid.NewGuid(),
            OccurredOn = _fixedNow
        };

        // Act & Assert
        var act = () => consumer.Consume(CreateConsumeContext(integrationEvent));
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Business logic failed");
    }

    [Fact]
    public async Task Consume_NewMessage_SetsCorrectEventTypeAndPayload()
    {
        // Arrange
        _dateTimeProvider.UtcNow.Returns(_fixedNow);
        using var dbContext = CreateDbContext();
        var consumer = new TestConsumer(dbContext, _dateTimeProvider, _logger);

        var integrationEvent = new TestIntegrationEvent(Guid.NewGuid(), "payload-test")
        {
            MessageId = Guid.NewGuid(),
            OccurredOn = _fixedNow
        };

        // Act
        await consumer.Consume(CreateConsumeContext(integrationEvent));

        // Assert
        var inbox = await dbContext.Set<InboxMessage>().SingleAsync();
        inbox.EventType.Should().Contain(nameof(TestIntegrationEvent));
        inbox.Payload.Should().Contain("payload-test");
        inbox.ReceivedOn.Should().Be(_fixedNow);
    }
}
