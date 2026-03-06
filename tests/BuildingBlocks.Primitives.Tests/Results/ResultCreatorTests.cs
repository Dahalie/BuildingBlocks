using BuildingBlocks.Primitives.Results;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Results;

public class ResultCreatorTests
{
    [Fact]
    public void Success_ReturnsSucceededResult()
    {
        var result = ResultCreator.Success();

        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Error.Should().Be(ErrorCreator.None());
    }

    [Fact]
    public void Success_WithMessage_SetsMessage()
    {
        var result = ResultCreator.Success("done");

        result.Message.Should().Be("done");
    }

    [Fact]
    public void Fail_ReturnsFailedResult()
    {
        var error = ErrorCreator.Business("code", "msg");

        var result = ResultCreator.Fail(error);

        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessGeneric_ReturnsDataResult()
    {
        var result = ResultCreator.Success(42);

        result.IsSucceeded.Should().BeTrue();
        result.DataOrDefault.Should().Be(42);
        result.HasData.Should().BeTrue();
    }

    [Fact]
    public void FailGeneric_ReturnsFailedDataResult()
    {
        var error = ErrorCreator.NotFound("Test");
        var result = ResultCreator.Fail<string>(error);

        result.IsFailed.Should().BeTrue();
        result.DataOrDefault.Should().BeNull();
        result.HasData.Should().BeFalse();
    }

    [Fact]
    public void NoContent_ReturnsSucceededNoContentResult()
    {
        var result = ResultCreator.NoContent();

        result.IsSucceeded.Should().BeTrue();
        result.DataOrDefault.Should().Be(NoContentDto.Instance);
    }

    [Fact]
    public void FromNullable_WithValue_ReturnsSuccess()
    {
        var result = ResultCreator.FromNullable("hello", () => ErrorCreator.NotFound("Test"));

        result.IsSucceeded.Should().BeTrue();
        result.DataOrDefault.Should().Be("hello");
    }

    [Fact]
    public void FromNullable_WithNull_ReturnsFail()
    {
        var result = ResultCreator.FromNullable<string>(null, () => ErrorCreator.NotFound("Test"));

        result.IsFailed.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void FromCollection_WithEmptyCollection_ReturnsFail()
    {
        var result = ResultCreator.FromCollection<List<int>>([], () => ErrorCreator.NotFound("Items"));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void FromCollection_WithItems_ReturnsSuccess()
    {
        var items = new List<int> { 1, 2, 3 };
        var result = ResultCreator.FromCollection(items, () => ErrorCreator.NotFound("Items"));

        result.IsSucceeded.Should().BeTrue();
        result.DataOrDefault.Should().HaveCount(3);
    }
}
