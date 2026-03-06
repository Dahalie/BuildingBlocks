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
        var statusCode = exception switch
                         {
                             ArgumentException           => StatusCodes.Status400BadRequest,
                             UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                             KeyNotFoundException        => StatusCodes.Status404NotFound,
                             _                           => StatusCodes.Status500InternalServerError
                         };

        var problemDetails = new ProblemDetails
        {
            Title = "UnhandledException", Detail = exception.Message, Status = statusCode, Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        context.Response.StatusCode = statusCode;

        await Results.Problem(problemDetails).ExecuteAsync(context);
    }
}