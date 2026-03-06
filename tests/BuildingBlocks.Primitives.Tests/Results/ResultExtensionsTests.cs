using BuildingBlocks.Primitives.Results;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Results;

public class ResultExtensionsTests
{
    [Fact]
    public void MapToFailedDataResult_OnFailed_MapsError()
    {
        var error = ErrorCreator.Business("code", "msg");
        var result = ResultCreator.Fail(error);

        var dataResult = result.MapToFailedDataResult<int>();

        dataResult.IsFailed.Should().BeTrue();
        dataResult.Error.Should().Be(error);
    }

    [Fact]
    public void MapToFailedDataResult_OnSuccess_ThrowsResultException()
    {
        var result = ResultCreator.Success();

        var act = () => result.MapToFailedDataResult<int>();

        act.Should().Throw<ResultException>();
    }

    [Fact]
    public void EnsureExists_WithEntity_ReturnsSuccess()
    {
        var entity = "found";

        var result = entity.EnsureExists(() => ErrorCreator.NotFound("Test"));

        result.IsSucceeded.Should().BeTrue();
        result.DataOrDefault.Should().Be("found");
    }

    [Fact]
    public void EnsureExists_WithNull_ReturnsFailed()
    {
        string? entity = null;

        var result = entity.EnsureExists(() => ErrorCreator.NotFound("Test"));

        result.IsFailed.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void ToResult_CreatesFailedDataResult()
    {
        var error = ErrorCreator.Business("code", "msg");

        var result = error.ToResult<int>();

        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ToNoContent_CreatesFailedNoContentResult()
    {
        var error = ErrorCreator.Validation("code", "msg");

        var result = error.ToNoContent();

        result.IsFailed.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.Validation);
    }
}
