using Autofac;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Repositories;
using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Behaviors;

public class TransactionalBehavior<TRequest, TResponse>(ILifetimeScope scope) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (typeof(IInboxCommand).IsAssignableFrom(typeof(TRequest)))
            return await next(cancellationToken);

        var (uowType, _) = UnitOfWorkResolver.FindTypes<TRequest, TResponse>(scope);

        if (uowType is null)
            return await next(cancellationToken);

        var unitOfWork = (IUnitOfWork)scope.Resolve(uowType);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await next(cancellationToken);

            if (result.IsSucceeded)
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            else
                await unitOfWork.RollbackTransactionAsync(cancellationToken);

            return result;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
