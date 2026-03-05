using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Requests;

public interface ICommand<TData> : IRequest<Result<TData>>;

public interface ICommand : ICommand<NoContentDto>;