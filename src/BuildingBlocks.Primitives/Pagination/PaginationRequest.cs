namespace BuildingBlocks.Primitives.Pagination;

public record PaginationRequest(int PageNumber = 1, int PageSize = 10)
{
    public int PageNumber { get; } = PageNumber < 1 ? 1 : PageNumber;
    public int PageSize   { get; } = PageSize   < 1 ? 10 : PageSize > 100 ? 100 : PageSize;

    public int Skip => (PageNumber - 1) * PageSize;
}