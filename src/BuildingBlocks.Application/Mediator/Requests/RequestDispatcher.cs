using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Requests;

internal sealed class RequestDispatcher(ISender sender) : IRequestDispatcher
{
    public Task<Result<TResponse>> DispatchAsync<TResponse>(IRequest<Result<TResponse>> request, CancellationToken cancellationToken = default)
        => sender.Send(request, cancellationToken);
}