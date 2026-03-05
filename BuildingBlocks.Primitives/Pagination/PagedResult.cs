namespace BuildingBlocks.Primitives.Pagination;

public record PagedResult<T>
{
    private PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items      = items;
        PageNumber = pageNumber;
        PageSize   = pageSize;
        TotalCount = totalCount;
    }

    public IReadOnlyList<T> Items       { get; }
    public int              PageNumber  { get; }
    public int              PageSize    { get; }
    public int              TotalCount  { get; }
    public int              TotalPages  => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool             HasPrevious => PageNumber > 1;
    public bool             HasNext     => PageNumber < TotalPages;

    public static PagedResult<T> Create(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
        => new(items, pageNumber, pageSize, totalCount);

    public static PagedResult<T> Empty(int pageNumber, int pageSize)
        => new([], pageNumber, pageSize, 0);
}