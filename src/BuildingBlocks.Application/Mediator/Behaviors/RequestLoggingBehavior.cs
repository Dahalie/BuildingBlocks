using System.Diagnostics;
using BuildingBlocks.Primitives.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Mediator.Behaviors;

public class RequestLoggingBehavior<TRequest, TResponse>(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private const int ThresholdMs = 500;

    private static readonly string RequestName = typeof(TRequest).Name;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing {RequestName}", RequestName);

        var sw       = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        sw.Stop();

        var elapsedMs = sw.ElapsedMilliseconds;

        if (response.IsFailed && response.Error.ErrorType is ErrorType.Exception)
            logger.LogWarning("Completed {RequestName} with error ({ElapsedMs}ms) — {ErrorCode}: {ErrorMessage}", RequestName, elapsedMs, response.Error.ErrorCode, response.Error.ErrorMessage);
        else if (elapsedMs > ThresholdMs)
            logger.LogWarning("Completed {RequestName} slowly ({ElapsedMs}ms)", RequestName, elapsedMs);
        else
            logger.LogInformation("Completed {RequestName} ({ElapsedMs}ms)", RequestName, elapsedMs);

        return response;
    }
}
