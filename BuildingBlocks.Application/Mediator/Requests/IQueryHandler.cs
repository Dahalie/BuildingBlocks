using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Requests;

public interface IQueryHandler<in TQuery, TData> : IRequestHandler<TQuery, Result<TData>>
    where TQuery : IQuery<TData>;