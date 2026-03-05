using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Mediator.Behaviors;

public class PerformanceLoggingBehavior<TRequest, TResponse>(ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int ThresholdMs = 500;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sw       = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        sw.Stop();

        if (sw.ElapsedMilliseconds > ThresholdMs)
            logger.LogWarning("Long running request: {RequestName} ({ElapsedMs}ms)", typeof(TRequest).Name, sw.ElapsedMilliseconds);

        return response;
    }
}