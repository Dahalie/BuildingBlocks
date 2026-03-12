using System.Collections.Concurrent;
using Autofac;
using Autofac.Core;
using BuildingBlocks.Application.Mediator.Requests;
using BuildingBlocks.Application.Repositories;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Behaviors;

internal static class UnitOfWorkResolver
{
    private static readonly ConcurrentDictionary<Type, (Type? UowType, Type? ContextType)> Cache = new();

    public static (Type? UowType, Type? ContextType) FindTypes<TRequest, TResponse>(ILifetimeScope scope)
    {
        return Cache.GetOrAdd(typeof(TRequest), _ => Resolve<TRequest, TResponse>(scope));
    }

    private static (Type? UowType, Type? ContextType) Resolve<TRequest, TResponse>(ILifetimeScope scope)
    {
        if (!typeof(ICommand<>).IsAssignableFromOpenGeneric(typeof(TRequest)))
            return (null, null);

        var handlerServiceType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));

        var registration = scope.ComponentRegistry
            .RegistrationsFor(new TypedService(handlerServiceType))
            .FirstOrDefault();

        if (registration is null)
            return (null, null);

        var uowType = registration.Activator.LimitType
            .GetConstructors()
            .SelectMany(c => c.GetParameters())
            .Select(p => p.ParameterType)
            .FirstOrDefault(t => t != typeof(IUnitOfWork) && typeof(IUnitOfWork).IsAssignableFrom(t));

        if (uowType is null)
            return (null, null);

        var contextType = uowType.IsGenericType
            ? uowType.GetGenericArguments().FirstOrDefault()
            : null;

        return (uowType, contextType);
    }

    internal static bool IsAssignableFromOpenGeneric(this Type openGeneric, Type type)
        => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
}
