using BuildingBlocks.Primitives.Exceptions;
using BuildingBlocks.Primitives.Results;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Exceptions;

public class CustomExceptionTests
{
    [Fact]
    public void DefaultConstructor_ErrorTypeIsException()
    {
        var ex = new CustomException();

        ex.ErrorType.Should().Be(ErrorType.Exception);
    }

    [Fact]
    public void MessageConstructor_ErrorTypeIsException()
    {
        var ex = new CustomException("something failed");

        ex.ErrorType.Should().Be(ErrorType.Exception);
        ex.Message.Should().Be("something failed");
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_ErrorTypeIsException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new CustomException("outer", inner);

        ex.ErrorType.Should().Be(ErrorType.Exception);
        ex.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void ErrorTypeConstructor_SetsErrorType()
    {
        var ex = new CustomException(ErrorType.NotFound);

        ex.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void MessageAndErrorTypeConstructor_SetsBoth()
    {
        var ex = new CustomException("user not found", ErrorType.NotFound);

        ex.ErrorType.Should().Be(ErrorType.NotFound);
        ex.Message.Should().Be("user not found");
    }

    [Fact]
    public void FullConstructor_SetsAll()
    {
        var inner = new Exception("cause");
        var ex = new CustomException("conflict", ErrorType.Conflict, inner);

        ex.ErrorType.Should().Be(ErrorType.Conflict);
        ex.Message.Should().Be("conflict");
        ex.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void ResultException_InheritsErrorType()
    {
        var ex = new ResultException("forbidden", ErrorType.Authorization);

        ex.ErrorType.Should().Be(ErrorType.Authorization);
        ex.Should().BeAssignableTo<CustomException>();
    }

    [Fact]
    public void ResultException_DefaultErrorTypeIsException()
    {
        var ex = new ResultException("something");

        ex.ErrorType.Should().Be(ErrorType.Exception);
    }
}
