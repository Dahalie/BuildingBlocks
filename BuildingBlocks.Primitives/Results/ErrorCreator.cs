using BuildingBlocks.Primitives.Types;

namespace BuildingBlocks.Primitives.Results;

public static class ErrorCreator
{
    internal static readonly IDictionary<string, object?> EmptyMetaData = new Dictionary<string, object?>();
    private static readonly  Error                        NoneInstance  = new(ErrorType.None, ErrorMessages.None, ErrorMessages.NoError);

    public static Error None()
        => NoneInstance;

    public static Error Create(ErrorType errorType, string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => new(errorType, errorCode, errorMessage, metaData);

    public static Error NotFound(string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => Create(ErrorType.NotFound, errorCode, errorMessage, metaData);

    public static Error NotFound(string instanceName)
        => NotFound(ErrorCodes.NotFound(instanceName), ErrorMessages.NotFound(instanceName));

    public static Error NotFound<TInstance>()
        => NotFound(typeof(TInstance).MapToReadableString());

    public static Error Conflict(string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => Create(ErrorType.Conflict, errorCode, errorMessage, metaData);

    public static Error Business(string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => Create(ErrorType.Business, errorCode, errorMessage, metaData);

    public static Error Exception(string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => Create(ErrorType.Exception, errorCode, errorMessage, metaData);

    public static Error Validation(string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => Create(ErrorType.Validation, errorCode, errorMessage, metaData);

    public static Error Authentication(string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => Create(ErrorType.Authentication, errorCode, errorMessage, metaData);

    public static Error Authorization(string errorCode, string errorMessage, IDictionary<string, object?>? metaData = null)
        => Create(ErrorType.Authorization, errorCode, errorMessage, metaData);
}