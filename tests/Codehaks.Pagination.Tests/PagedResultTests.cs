using Codehaks.Pagination;
using Xunit;

namespace Codehaks.Pagination.Tests;

public class PagedResultTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(100, 10, 10)]
    [InlineData(105, 10, 11)]   // regression: ceiling, not truncating integer division
    [InlineData(1, 10, 1)]
    [InlineData(95, 10, 10)]
    public void Total_pages_rounds_up(int totalCount, int pageSize, int expected)
    {
        var result = new PagedResult<int>([], 1, pageSize, totalCount);

        Assert.Equal(expected, result.TotalPages);
    }

    [Fact]
    public void Has_previous_page_is_false_on_first_page()
    {
        var result = new PagedResult<int>([], 1, 10, 100);

        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void Has_next_page_is_false_on_last_page()
    {
        var result = new PagedResult<int>([], 10, 10, 100);

        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }
}
