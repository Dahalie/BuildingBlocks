using BuildingBlocks.Primitives.Results;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult(this Result result)
        => result.IsSucceeded ? Results.Ok(result) : ToProblemResult(result.Error);

    public static IResult ToNoContentApiResult(this Result result)
        => result.IsSucceeded ? Results.NoContent() : ToProblemResult(result.Error);

    private static IResult ToProblemResult(Error error)
    {
        var statusCode = error.ErrorType switch
                         {
                             ErrorType.Validation     => StatusCodes.Status400BadRequest,
                             ErrorType.Authentication => StatusCodes.Status401Unauthorized,
                             ErrorType.Authorization  => StatusCodes.Status403Forbidden,
                             ErrorType.NotFound       => StatusCodes.Status404NotFound,
                             ErrorType.Conflict       => StatusCodes.Status409Conflict,
                             ErrorType.Business       => StatusCodes.Status422UnprocessableEntity,
                             ErrorType.Exception      => StatusCodes.Status500InternalServerError,
                             _                        => StatusCodes.Status500InternalServerError
                         };

        return Results.Problem(title: error.ErrorCode, detail: error.ErrorMessage, statusCode: statusCode,
            extensions: error.HasMetaData ? new Dictionary<string, object?> { ["metadata"] = error.MetaData } : null);
    }

    extension<TData>(Result<TData> result)
    {
        public IResult ToApiResult()
            => result.IsSucceeded ? Results.Ok((object?)result.DataOrDefault) : ToProblemResult(result.Error);

        public IResult ToCreatedApiResult(string uri)
            => result.IsSucceeded ? Results.Created(uri, (object?)result.DataOrDefault) : ToProblemResult(result.Error);

        public IResult ToAcceptedApiResult()
            => result.IsSucceeded ? Results.Accepted(value: (object?)result.DataOrDefault) : ToProblemResult(result.Error);
    }
}