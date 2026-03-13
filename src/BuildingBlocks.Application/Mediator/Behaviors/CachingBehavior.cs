using System.Reflection;
using System.Text.Json;
using BuildingBlocks.Application.Mediator.Requests;
using BuildingBlocks.Primitives.Results;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Mediator.Behaviors;

public class CachingBehavior<TRequest, TResponse>(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableQuery
    where TResponse : Result
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private static readonly Type? DataType = typeof(TResponse).IsGenericType
        ? typeof(TResponse).GetGenericArguments()[0]
        : null;

    private static readonly PropertyInfo? DataProperty = DataType is not null
        ? typeof(TResponse).GetProperty(nameof(Result<object>.DataOrDefault))
        : null;

    private static readonly MethodInfo? SuccessMethod = DataType is not null
        ? typeof(ResultCreator).GetMethods().First(m => m is { Name: nameof(ResultCreator.Success), IsGenericMethod: true } && m.GetParameters().Length == 2).MakeGenericMethod(DataType)
        : null;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cached = await cache.GetStringAsync(request.CacheKey, cancellationToken);

        if (cached is not null && SuccessMethod is not null)
        {
            logger.LogDebug("Cache hit for {CacheKey}", request.CacheKey);
            var data = JsonSerializer.Deserialize(cached, DataType!);
            return (TResponse)SuccessMethod.Invoke(null, [data, null])!;
        }

        var response = await next(cancellationToken);

        if (response.IsSucceeded && DataProperty is not null)
        {
            var data = DataProperty.GetValue(response);
            var json = JsonSerializer.Serialize(data, DataType!);
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = request.Expiration ?? DefaultExpiration };

            await cache.SetStringAsync(request.CacheKey, json, options, cancellationToken);

            logger.LogDebug("Cache set for {CacheKey}", request.CacheKey);
        }

        return response;
    }
}
