using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Requests;

public interface ICommandHandler<in TCommand, TData> : IRequestHandler<TCommand, Result<TData>>
    where TCommand : ICommand<TData>;

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, NoContentDto>
    where TCommand : ICommand;