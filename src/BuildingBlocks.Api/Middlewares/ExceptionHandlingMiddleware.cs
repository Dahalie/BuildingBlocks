using BuildingBlocks.Primitives.Exceptions;
using BuildingBlocks.Primitives.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = MapToHttpStatusCode(exception);

        var problemDetails = new ProblemDetails
        {
            Title    = "UnhandledException",
            Detail   = exception.Message,
            Status   = statusCode,
            Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        context.Response.StatusCode = statusCode;

        await Results.Problem(problemDetails).ExecuteAsync(context);
    }

    internal static int MapToHttpStatusCode(Exception exception)
    {
        if (exception is CustomException customException)
            return MapErrorType(customException.ErrorType);

        return exception switch
        {
            ArgumentException           => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException        => StatusCodes.Status404NotFound,
            _                           => StatusCodes.Status500InternalServerError
        };
    }

    internal static int MapErrorType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation     => StatusCodes.Status400BadRequest,
            ErrorType.Authentication => StatusCodes.Status401Unauthorized,
            ErrorType.Authorization  => StatusCodes.Status403Forbidden,
            ErrorType.NotFound       => StatusCodes.Status404NotFound,
            ErrorType.Conflict       => StatusCodes.Status409Conflict,
            ErrorType.Business       => StatusCodes.Status422UnprocessableEntity,
            _                        => StatusCodes.Status500InternalServerError
        };
    }
}