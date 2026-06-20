using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Codehaks.Pagination.Sample.Data;

/// <summary>
/// Creates the SQLite database (if needed) and seeds enough rows for pagination to be visible.
/// The sample uses a throwaway SQLite file rebuilt on first run, so the database is not committed.
/// </summary>
public static class DbSeeder
{
    private const int SeedCount = 95;

    public static async Task SeedAsync(PeopleDbContext db, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(db);

        await db.Database.EnsureCreatedAsync(ct);

        if (await db.Users.AnyAsync(ct))
        {
            return;
        }

        var users = Enumerable.Range(1, SeedCount).Select(i => new User
        {
            Givenname = "Given" + i.ToString(CultureInfo.InvariantCulture),
            Maidenname = "Family" + i.ToString(CultureInfo.InvariantCulture),
            Age = 18 + (i % 50),
        });

        await db.Users.AddRangeAsync(users, ct);
        await db.SaveChangesAsync(ct);
    }
}
