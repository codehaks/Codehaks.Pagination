using System.Linq;

namespace Codehaks.Pagination;

/// <summary>
/// Paging helpers over <see cref="IQueryable{T}"/>. These compose with the rest of a LINQ
/// pipeline so the query is translated and executed by the underlying provider.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Applies <c>Skip</c>/<c>Take</c> for the given page without materializing the query.
    /// </summary>
    /// <param name="source">The query to page.</param>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pageNumber"/> is less than 1 or <paramref name="pageSize"/> is not positive.
    /// </exception>
    public static IQueryable<T> Page<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        return source
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
    }

    /// <summary>
    /// Materializes a single page into a <see cref="PagedResult{T}"/>, issuing a count query and
    /// a page query.
    /// </summary>
    /// <param name="source">The query to page.</param>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pageNumber"/> is less than 1 or <paramref name="pageSize"/> is not positive.
    /// </exception>
    public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        var totalCount = source.Count();
        var items = source.Page(pageNumber, pageSize).ToList();

        return new PagedResult<T>(items, pageNumber, pageSize, totalCount);
    }
}
