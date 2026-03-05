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

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cached = await cache.GetStringAsync(request.CacheKey, cancellationToken);

        if (cached is not null)
        {
            logger.LogDebug("Cache hit for {CacheKey}", request.CacheKey);
            return JsonSerializer.Deserialize<TResponse>(cached)!;
        }

        var response = await next(cancellationToken);

        if (response.IsSucceeded)
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = request.Expiration ?? DefaultExpiration };

            await cache.SetStringAsync(request.CacheKey, JsonSerializer.Serialize(response), options, cancellationToken);

            logger.LogDebug("Cache set for {CacheKey}", request.CacheKey);
        }

        return response;
    }
}