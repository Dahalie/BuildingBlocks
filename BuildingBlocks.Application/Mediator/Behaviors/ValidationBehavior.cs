using BuildingBlocks.Primitives.Results;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Application.Mediator.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)))).SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var metadata = failures.GroupBy(f => f.PropertyName).ToDictionary(g => g.Key, g => (object?)g.Select(f => f.ErrorMessage).ToArray());

        var error = ErrorCreator.Validation(ErrorCodes.Validation(typeof(TRequest).Name), ErrorMessages.Validation, metadata);

        return CreateFailedResult(error);
    }

    private static TResponse CreateFailedResult(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)ResultCreator.Fail(error);

        var dataType = typeof(TResponse).GetGenericArguments()[0];

        var failMethod = typeof(ResultCreator).GetMethods().First(m => m is { Name: "Fail", IsGenericMethod: true } && m.GetParameters().Length == 2).MakeGenericMethod(dataType);

        return (TResponse)failMethod.Invoke(null, [error, null])!;
    }
}