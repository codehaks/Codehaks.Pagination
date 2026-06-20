namespace Codehaks.Pagination;

/// <summary>
/// One page of results plus the paging metadata needed to render a pagination control.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <param name="Items">The items on the current page.</param>
/// <param name="PageNumber">The current (1-based) page number.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="TotalCount">The total number of items across all pages.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    /// <summary>The total number of pages, rounded up (ceiling division).</summary>
    public int TotalPages =>
        PageSize <= 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;

    /// <summary>Whether a previous page exists.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Whether a next page exists.</summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
