using Codehaks.Pagination;
using Codehaks.Pagination.Sample.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Codehaks.Pagination.Sample.Pages;

public class IndexModel : PageModel
{
    private const int DefaultPageSize = 10;

    private readonly PeopleDbContext _db;

    public IndexModel(PeopleDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<User> UserList { get; private set; } = [];

    public int PageSize { get; private set; } = DefaultPageSize;

    public int TotalPages { get; private set; }

    public int PageNumber { get; private set; }

    public async Task OnGetAsync(int number = 1, CancellationToken ct = default)
    {
        if (number < 1)
        {
            number = 1;
        }

        var totalCount = await _db.Users.CountAsync(ct);

        // Demonstrates the library's IQueryable paging helper, materialized async by EF Core.
        UserList = await _db.Users
            .OrderBy(u => u.Number)
            .Page(number, DefaultPageSize)
            .ToListAsync(ct);

        PageNumber = number;
        TotalPages = (totalCount + DefaultPageSize - 1) / DefaultPageSize;
    }
}
