using BuildingBlocks.Application.Mediator.Requests;

namespace BuildingBlocks.Application.Messaging;

public interface IInboxCommand : ICommand
{
    Guid MessageId { get; }
}
