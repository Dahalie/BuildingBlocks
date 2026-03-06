using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Requests;

public interface IQuery<TData> : IRequest<Result<TData>>;