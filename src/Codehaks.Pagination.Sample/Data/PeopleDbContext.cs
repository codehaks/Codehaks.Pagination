using Microsoft.EntityFrameworkCore;

namespace Codehaks.Pagination.Sample.Data;

public class PeopleDbContext : DbContext
{
    public PeopleDbContext(DbContextOptions<PeopleDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}
