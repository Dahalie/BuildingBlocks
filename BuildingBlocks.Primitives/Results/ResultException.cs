using BuildingBlocks.Primitives.Exceptions;

namespace BuildingBlocks.Primitives.Results;

public class ResultException : CustomException
{
    public ResultException()
    {
    }

    public ResultException(string message) : base(message)
    {
    }

    public ResultException(string message, Exception innerException) : base(message, innerException)
    {
    }
}