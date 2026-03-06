namespace BuildingBlocks.Primitives.Results;

public record Error
{
    internal Error(ErrorType errorType, string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
    {
        ErrorType    = errorType;
        ErrorCode    = errorCode;
        ErrorMessage = errorMessage;
        MetaData     = metaData ?? ErrorCreator.EmptyMetaData;
    }

    public ErrorType                    ErrorType    { get; }
    public string                       ErrorCode    { get; }
    public string                       ErrorMessage { get; }
    public IDictionary<string, object?> MetaData     { get; }

    public bool HasMetaData => MetaData.Count > 0;

    public override string ToString()
        => $"{ErrorCode} ({ErrorType}): {ErrorMessage}";

    public static implicit operator Result(Error error)
        => ResultCreator.Fail(error);
}