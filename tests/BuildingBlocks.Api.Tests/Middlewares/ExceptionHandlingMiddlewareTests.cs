using BuildingBlocks.Api.Middlewares;
using BuildingBlocks.Primitives.Exceptions;
using BuildingBlocks.Primitives.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Tests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    // ErrorType mapping tests

    [Theory]
    [InlineData(ErrorType.Validation,     StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.Authentication, StatusCodes.Status401Unauthorized)]
    [InlineData(ErrorType.Authorization,  StatusCodes.Status403Forbidden)]
    [InlineData(ErrorType.NotFound,       StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict,       StatusCodes.Status409Conflict)]
    [InlineData(ErrorType.Business,       StatusCodes.Status422UnprocessableEntity)]
    [InlineData(ErrorType.Exception,      StatusCodes.Status500InternalServerError)]
    public void MapToHttpStatusCode_CustomException_MapsErrorType(ErrorType errorType, int expectedStatusCode)
    {
        var exception = new CustomException("test", errorType);

        var result = ExceptionHandlingMiddleware.MapToHttpStatusCode(exception);

        result.Should().Be(expectedStatusCode);
    }

    [Fact]
    public void MapToHttpStatusCode_CustomExceptionWithoutErrorType_MapsToInternalServerError()
    {
        var exception = new CustomException("unexpected");

        var result = ExceptionHandlingMiddleware.MapToHttpStatusCode(exception);

        result.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void MapToHttpStatusCode_ResultException_MapsErrorType()
    {
        var exception = new ResultException("not found", ErrorType.NotFound);

        var result = ExceptionHandlingMiddleware.MapToHttpStatusCode(exception);

        result.Should().Be(StatusCodes.Status404NotFound);
    }

    // Standard exception fallback tests

    [Fact]
    public void MapToHttpStatusCode_ArgumentException_Maps400()
    {
        var result = ExceptionHandlingMiddleware.MapToHttpStatusCode(new ArgumentException("bad"));

        result.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public void MapToHttpStatusCode_UnauthorizedAccessException_Maps401()
    {
        var result = ExceptionHandlingMiddleware.MapToHttpStatusCode(new UnauthorizedAccessException());

        result.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void MapToHttpStatusCode_KeyNotFoundException_Maps404()
    {
        var result = ExceptionHandlingMiddleware.MapToHttpStatusCode(new KeyNotFoundException());

        result.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void MapToHttpStatusCode_UnknownException_Maps500()
    {
        var result = ExceptionHandlingMiddleware.MapToHttpStatusCode(new Exception("boom"));

        result.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
