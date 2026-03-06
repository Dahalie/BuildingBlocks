using BuildingBlocks.Primitives.Pagination;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Pagination;

public class PaginationRequestTests
{
    [Fact]
    public void Defaults_AreCorrect()
    {
        var request = new PaginationRequest();

        request.PageNumber.Should().Be(1);
        request.PageSize.Should().Be(10);
    }

    [Fact]
    public void PageNumber_BelowOne_ClampsToOne()
    {
        var request = new PaginationRequest(PageNumber: -5);

        request.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PageSize_BelowOne_ClampsToTen()
    {
        var request = new PaginationRequest(PageSize: 0);

        request.PageSize.Should().Be(10);
    }

    [Fact]
    public void PageSize_AboveHundred_ClampsToHundred()
    {
        var request = new PaginationRequest(PageSize: 500);

        request.PageSize.Should().Be(100);
    }

    [Fact]
    public void Skip_CalculatesCorrectly()
    {
        var request = new PaginationRequest(PageNumber: 3, PageSize: 20);

        request.Skip.Should().Be(40); // (3-1) * 20
    }

    [Fact]
    public void ValidValues_ArePreserved()
    {
        var request = new PaginationRequest(PageNumber: 5, PageSize: 50);

        request.PageNumber.Should().Be(5);
        request.PageSize.Should().Be(50);
    }
}
