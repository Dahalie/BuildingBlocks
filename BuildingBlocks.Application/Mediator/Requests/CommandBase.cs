using BuildingBlocks.Primitives.Results;

namespace BuildingBlocks.Application.Mediator.Requests;

public abstract record CommandBase<TResponse> : ICommand<TResponse>;

public abstract record CommandBase : CommandBase<NoContentDto>, ICommand;