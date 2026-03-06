using BuildingBlocks.Primitives.Results;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Results;

public class ErrorCreatorTests
{
    [Fact]
    public void None_ReturnsSingletonInstance()
    {
        var a = ErrorCreator.None();
        var b = ErrorCreator.None();

        a.Should().BeSameAs(b);
        a.ErrorType.Should().Be(ErrorType.None);
    }

    [Theory]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Business)]
    [InlineData(ErrorType.Exception)]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.Authentication)]
    [InlineData(ErrorType.Authorization)]
    public void FactoryMethods_SetCorrectErrorType(ErrorType expectedType)
    {
        var error = expectedType switch
        {
            ErrorType.NotFound       => ErrorCreator.NotFound("code", "msg"),
            ErrorType.Conflict       => ErrorCreator.Conflict("code", "msg"),
            ErrorType.Business       => ErrorCreator.Business("code", "msg"),
            ErrorType.Exception      => ErrorCreator.Exception("code", "msg"),
            ErrorType.Validation     => ErrorCreator.Validation("code", "msg"),
            ErrorType.Authentication => ErrorCreator.Authentication("code", "msg"),
            ErrorType.Authorization  => ErrorCreator.Authorization("code", "msg"),
            _                        => throw new ArgumentOutOfRangeException()
        };

        error.ErrorType.Should().Be(expectedType);
        error.ErrorCode.Should().Be("code");
        error.ErrorMessage.Should().Be("msg");
    }

    [Fact]
    public void NotFound_GenericOverload_UsesTypeName()
    {
        var error = ErrorCreator.NotFound<string>();

        error.ErrorType.Should().Be(ErrorType.NotFound);
        error.ErrorCode.Should().Contain("String");
    }

    [Fact]
    public void Create_WithMetaData_SetsMetaData()
    {
        var meta = new Dictionary<string, object?> { ["key"] = "value" };
        var error = ErrorCreator.Create(ErrorType.Business, "code", "msg", meta);

        error.HasMetaData.Should().BeTrue();
        error.MetaData["key"].Should().Be("value");
    }

    [Fact]
    public void Error_ImplicitConversionToResult_ReturnsFailed()
    {
        Result result = ErrorCreator.Business("code", "msg");

        result.IsFailed.Should().BeTrue();
        result.Error.ErrorCode.Should().Be("code");
    }
}
