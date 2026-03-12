using Autofac;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Repositories;
using BuildingBlocks.Primitives.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Mediator.Behaviors;

public class InboxBehavior<TRequest, TResponse>(
    ILifetimeScope scope,
    ILogger<InboxBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IInboxCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var handlerType = typeof(TRequest).FullName!;

        var (uowType, contextType) = UnitOfWorkResolver.FindTypes<TRequest, TResponse>(scope);

        if (uowType is null || contextType is null)
            return await next(cancellationToken);

        var unitOfWork = (IUnitOfWork)scope.Resolve(uowType);
        var inboxStore = scope.ResolveKeyed<IInboxStore>(contextType);

        if (await inboxStore.ExistsAsync(request.MessageId, handlerType, cancellationToken))
        {
            logger.LogInformation(
                "Duplicate message {MessageId} skipped for {Handler}",
                request.MessageId, typeof(TRequest).Name);

            return (TResponse)(object)ResultCreator.NoContent("Already processed");
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await inboxStore.RecordAsync(
                request.MessageId, typeof(TRequest).FullName!, handlerType, cancellationToken);

            var result = await next(cancellationToken);

            if (result.IsSucceeded)
            {
                await inboxStore.MarkProcessedAsync(request.MessageId, handlerType, cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            else
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
            }

            return result;
        }
        catch (InboxDuplicateMessageException)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);

            logger.LogInformation(
                "Concurrent duplicate message {MessageId} skipped for {Handler}",
                request.MessageId, typeof(TRequest).Name);

            return (TResponse)(object)ResultCreator.NoContent("Already processed");
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
