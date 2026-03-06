using System.Collections.Concurrent;
using Autofac;
using Autofac.Core;
using BuildingBlocks.Application.Mediator.Requests;
using BuildingBlocks.Application.Repositories;
using BuildingBlocks.Primitives.Results;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Behaviors;

public class TransactionalBehavior<TRequest, TResponse>(ILifetimeScope scope) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private static readonly ConcurrentDictionary<Type, Type?> UnitOfWorkCache = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var uowType = UnitOfWorkCache.GetOrAdd(typeof(TRequest), _ => ResolveUnitOfWorkType());

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

    private Type? ResolveUnitOfWorkType()
    {
        if (!typeof(ICommand<>).IsAssignableFromOpenGeneric(typeof(TRequest)))
            return null;

        var handlerServiceType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));

        var registration = scope.ComponentRegistry
            .RegistrationsFor(new TypedService(handlerServiceType))
            .FirstOrDefault();

        if (registration is null)
            return null;

        return registration.Activator.LimitType
            .GetConstructors()
            .SelectMany(c => c.GetParameters())
            .Select(p => p.ParameterType)
            .FirstOrDefault(t => t != typeof(IUnitOfWork) && typeof(IUnitOfWork).IsAssignableFrom(t));
    }
}

file static class TypeExtensions
{
    public static bool IsAssignableFromOpenGeneric(this Type openGeneric, Type type)
        => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
}
