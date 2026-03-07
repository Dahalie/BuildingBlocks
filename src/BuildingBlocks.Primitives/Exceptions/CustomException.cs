using BuildingBlocks.Primitives.Results;

namespace BuildingBlocks.Primitives.Exceptions;

public class CustomException : Exception
{
    public ErrorType ErrorType { get; }

    public CustomException() : this(ErrorType.Exception)
    {
    }

    public CustomException(string message) : this(message, ErrorType.Exception)
    {
    }

    public CustomException(string message, Exception innerException) : this(message, ErrorType.Exception, innerException)
    {
    }

    public CustomException(ErrorType errorType)
    {
        ErrorType = errorType;
    }

    public CustomException(string message, ErrorType errorType) : base(message)
    {
        ErrorType = errorType;
    }

    public CustomException(string message, ErrorType errorType, Exception innerException) : base(message, innerException)
    {
        ErrorType = errorType;
    }
}