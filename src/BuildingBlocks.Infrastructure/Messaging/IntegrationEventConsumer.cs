using BuildingBlocks.Application.Mediator.Requests;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Contracts.Messaging;
using MassTransit;

namespace BuildingBlocks.Infrastructure.Messaging;

public abstract class IntegrationEventConsumer<TEvent>(IRequestDispatcher dispatcher) : IConsumer<TEvent>
    where TEvent : class, IIntegrationEvent
{
    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var command = MapToCommand(context.Message);

        var result = await dispatcher.DispatchAsync(command, context.CancellationToken);

        if (result.IsFailed)
            throw new IntegrationEventProcessingException(result.Error);
    }

    protected abstract IInboxCommand MapToCommand(TEvent message);
}
