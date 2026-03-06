using System.Collections;

namespace BuildingBlocks.Primitives.Results;

public static class ResultCreator
{
    public static Result Create(bool isSucceeded, Error error, string? message = null)
        => new(isSucceeded, error, message);

    public static Result<TData> Create<TData>(bool isSucceeded, Error error, TData? data, string? message = null)
        => new(isSucceeded, error, data, message);


    public static Result Success(string? message = null)
        => Create(true, ErrorCreator.None(), message);

    public static Result Fail(Error error, string? message = null)
        => Create(false, error, message);


    public static Result<TData> Success<TData>(TData data, string? message = null)
        => Create<TData>(true, ErrorCreator.None(), data, message);

    public static Result<TData> Fail<TData>(Error error, string? message = null)
        => Create<TData>(false, error, default, message);

    public static Result<NoContentDto> NoContent(string? message = null)
        => Success(NoContentDto.Instance, message);


    public static Result<TData> FromNullable<TData>(TData? data, Func<Error> errorFunc, string? message = null)
        => data is not null ? Success(data, message) : Fail<TData>(errorFunc(), message);

    public static Result<TData> FromCollection<TData>(TData? data, Func<Error> errorFunc, string? message = null)
        where TData : ICollection
    {
        if (data is null || data.Count == 0) return Fail<TData>(errorFunc(), message);

        return Success(data, message);
    }
}