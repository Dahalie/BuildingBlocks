using BuildingBlocks.Primitives.Pagination;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Pagination;

public class PagedResultTests
{
    [Fact]
    public void Create_SetsProperties()
    {
        var items = new[] { 1, 2, 3 };

        var result = PagedResult<int>.Create(items, pageNumber: 2, pageSize: 3, totalCount: 10);

        result.Items.Should().BeEquivalentTo(items);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(3);
        result.TotalCount.Should().Be(10);
    }

    [Fact]
    public void TotalPages_CalculatesCorrectly()
    {
        var result = PagedResult<int>.Create([], pageNumber: 1, pageSize: 3, totalCount: 10);

        result.TotalPages.Should().Be(4); // ceil(10/3) = 4
    }

    [Fact]
    public void HasPrevious_FirstPage_ReturnsFalse()
    {
        var result = PagedResult<int>.Create([], pageNumber: 1, pageSize: 10, totalCount: 50);

        result.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void HasPrevious_SecondPage_ReturnsTrue()
    {
        var result = PagedResult<int>.Create([], pageNumber: 2, pageSize: 10, totalCount: 50);

        result.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public void HasNext_LastPage_ReturnsFalse()
    {
        var result = PagedResult<int>.Create([], pageNumber: 5, pageSize: 10, totalCount: 50);

        result.HasNext.Should().BeFalse();
    }

    [Fact]
    public void HasNext_NotLastPage_ReturnsTrue()
    {
        var result = PagedResult<int>.Create([], pageNumber: 3, pageSize: 10, totalCount: 50);

        result.HasNext.Should().BeTrue();
    }

    [Fact]
    public void Empty_ReturnsEmptyResult()
    {
        var result = PagedResult<string>.Empty(1, 10);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasNext.Should().BeFalse();
    }
}
