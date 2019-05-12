using Microsoft.EntityFrameworkCore;

namespace PeopleApp.Data
{
    public class PeopleDbContext : DbContext
    {
        public PeopleDbContext(DbContextOptions<PeopleDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<User>().HasQueryFilter(u => u.Age > 30);
            base.OnModelCreating(builder);
        }
    }
}