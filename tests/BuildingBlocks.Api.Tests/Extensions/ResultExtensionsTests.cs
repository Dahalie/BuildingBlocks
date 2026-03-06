using BuildingBlocks.Api.Extensions;
using BuildingBlocks.Primitives.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BuildingBlocks.Api.Tests.Extensions;

public class ResultExtensionsTests
{
    [Fact]
    public void ToApiResult_OnSuccess_ReturnsOk()
    {
        var result = ResultCreator.Success();

        var apiResult = result.ToApiResult();

        apiResult.Should().BeOfType<Ok<Result>>();
    }

    [Fact]
    public void ToApiResult_OnFailed_ReturnsProblem()
    {
        var result = ResultCreator.Fail(ErrorCreator.NotFound("Test"));

        var apiResult = result.ToApiResult();

        apiResult.Should().BeOfType<ProblemHttpResult>();
    }

    [Fact]
    public void ToNoContentApiResult_OnSuccess_ReturnsNoContent()
    {
        var result = ResultCreator.Success();

        var apiResult = result.ToNoContentApiResult();

        apiResult.Should().BeAssignableTo<IStatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public void ToApiResult_GenericOnSuccess_ReturnsOk()
    {
        var result = ResultCreator.Success<string>("hello");

        var apiResult = result.ToApiResult();

        apiResult.Should().BeOfType<Ok<object>>();
    }

    [Fact]
    public void ToApiResult_GenericOnFailed_ReturnsProblem()
    {
        var result = ResultCreator.Fail<string>(ErrorCreator.Business("code", "msg"));

        var apiResult = result.ToApiResult();

        apiResult.Should().BeOfType<ProblemHttpResult>();
    }
}
