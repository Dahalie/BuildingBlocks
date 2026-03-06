using BuildingBlocks.Primitives.Results;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Results;

public class ResultTests
{
    [Fact]
    public void DataOrThrow_OnSuccess_ReturnsData()
    {
        var result = ResultCreator.Success<string>("data");

        result.DataOrThrow.Should().Be("data");
    }

    [Fact]
    public void DataOrThrow_OnFailed_ThrowsResultException()
    {
        var result = ResultCreator.Fail<string>(ErrorCreator.Business("code", "msg"));

        var act = () => result.DataOrThrow;

        act.Should().Throw<ResultException>();
    }

    [Fact]
    public void TryGetData_OnSuccess_ReturnsTrueWithData()
    {
        var result = ResultCreator.Success(42);

        var success = result.TryGetData(out var data);

        success.Should().BeTrue();
        data.Should().Be(42);
    }

    [Fact]
    public void TryGetData_OnFailed_ReturnsFalse()
    {
        var result = ResultCreator.Fail<int>(ErrorCreator.Business("code", "msg"));

        var success = result.TryGetData(out var data);

        success.Should().BeFalse();
        data.Should().Be(default(int));
    }

    [Fact]
    public void DataOrDefault_OnSuccess_ReturnsData()
    {
        var result = ResultCreator.Success<string>("hello");

        result.DataOrDefault.Should().Be("hello");
    }

    [Fact]
    public void DataOrDefault_OnFailed_ReturnsDefault()
    {
        var result = ResultCreator.Fail<string>(ErrorCreator.Business("code", "msg"));

        result.DataOrDefault.Should().BeNull();
    }

    [Fact]
    public void Constructor_SucceededWithError_ThrowsResultException()
    {
        var act = () => ResultCreator.Create(true, ErrorCreator.Business("code", "msg"));

        act.Should().Throw<ResultException>();
    }

    [Fact]
    public void Constructor_FailedWithNoneError_ThrowsResultException()
    {
        var act = () => ResultCreator.Create(false, ErrorCreator.None());

        act.Should().Throw<ResultException>();
    }

    [Fact]
    public void Constructor_SucceededDataResultWithNullData_ThrowsResultException()
    {
        var act = () => ResultCreator.Create<string>(true, ErrorCreator.None(), null);

        act.Should().Throw<ResultException>();
    }

    [Fact]
    public void IsFailed_IsInverseOfIsSucceeded()
    {
        var success = ResultCreator.Success();
        var failed = ResultCreator.Fail(ErrorCreator.Business("code", "msg"));

        success.IsFailed.Should().BeFalse();
        failed.IsFailed.Should().BeTrue();
    }
}
