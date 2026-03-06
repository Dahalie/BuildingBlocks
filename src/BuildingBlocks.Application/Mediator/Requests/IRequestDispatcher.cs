using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Requests;

public interface IRequestDispatcher
{
    Task<Result<TResponse>> DispatchAsync<TResponse>(IRequest<Result<TResponse>> request, CancellationToken cancellationToken = default);
}