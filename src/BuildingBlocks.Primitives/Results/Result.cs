using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Primitives.Results;

public record Result
{
    public Result(bool isSucceeded, Error error, string? message)
    {
        ResultValidator.Validate(isSucceeded, error);

        IsSucceeded = isSucceeded;
        Error       = error;
        Message     = message;
    }

    public bool IsSucceeded { get; }

    [JsonIgnore] public bool IsFailed => !IsSucceeded;

    public Error Error { get; }

    public string? Message { get; }
}

public record Result<TData> : Result
{
    private readonly TData? _data;

    internal Result(bool isSucceeded, Error error, TData? data, string? message) : base(isSucceeded, error, message)
    {
        ResultValidator.Validate(isSucceeded, data);

        _data = data;
    }


    [JsonIgnore] public bool HasData => _data is not null;

    public TData? DataOrDefault => IsSucceeded ? _data : default;
    public TData  DataOrThrow   => IsFailed ? throw new ResultException(ResultMessages.FailedResultCannotAccessData) : _data!;

    public bool TryGetData([NotNullWhen(true)] out TData? data)
    {
        if (IsSucceeded)
        {
            data = _data!;
            return true;
        }

        data = default;
        return false;
    }
}