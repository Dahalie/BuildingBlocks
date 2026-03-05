namespace BuildingBlocks.Primitives.Results;

public static class ResultExtensions
{
    public static Result<TData> MapToFailedDataResult<TData>(this Result result)
    {
        if (result.IsSucceeded)
            throw new ResultException("Succeeded results cannot be converted without data. Use ResultCreator.Success<TData>(data) instead.");

        return ResultCreator.Fail<TData>(result.Error, result.Message);
    }

    public static Result<T> EnsureExists<T>(this T? entity, Func<Error> errorFactory)
        where T : class
        => entity is not null ? ResultCreator.Success(entity) : ResultCreator.Fail<T>(errorFactory());

    public static Result<T> ToResult<T>(this Error error, string? message = null)
        => ResultCreator.Fail<T>(error, message);

    public static Result<NoContentDto> ToNoContent(this Error error, string? message = null)
        => ResultCreator.Fail<NoContentDto>(error, message);
}