using Codehaks.Pagination.Sample.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddRazorPages();
builder.Services.AddDbContext<PeopleDbContext>(options =>
    options.UseSqlite("Data Source=People.sqlite"));

var app = builder.Build();

// seed the throwaway SQLite database on startup (sample only)
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PeopleDbContext>();
    await DbSeeder.SeedAsync(db);
}

// pipeline
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

await app.RunAsync();
