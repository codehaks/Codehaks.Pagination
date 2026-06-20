using System;
using System.Linq;
using Codehaks.Pagination;
using Xunit;

namespace Codehaks.Pagination.Tests;

public class PaginationExtensionsTests
{
    private static IQueryable<int> Source(int count) =>
        Enumerable.Range(1, count).AsQueryable();

    [Fact]
    public void Page_returns_the_requested_slice()
    {
        var page = Source(95).Page(2, 10).ToList();

        Assert.Equal(Enumerable.Range(11, 10), page);
    }

    [Fact]
    public void Page_last_partial_page_returns_remainder()
    {
        var page = Source(95).Page(10, 10).ToList();

        Assert.Equal([91, 92, 93, 94, 95], page);
    }

    [Fact]
    public void To_paged_result_captures_total_count_and_items()
    {
        var result = Source(95).ToPagedResult(2, 10);

        Assert.Equal(95, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(10, result.TotalPages);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(11, result.Items[0]);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    public void Page_rejects_page_number_below_one(int pageNumber, int pageSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Source(10).Page(pageNumber, pageSize).ToList());
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    public void Page_rejects_non_positive_page_size(int pageNumber, int pageSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Source(10).Page(pageNumber, pageSize).ToList());
    }

    [Fact]
    public void Page_rejects_null_source()
    {
        IQueryable<int> source = null!;

        Assert.Throws<ArgumentNullException>(() => source.Page(1, 10).ToList());
    }
}
