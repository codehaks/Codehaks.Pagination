# Examples by use case

Every example assumes you've installed the package and registered the Tag Helper
(`@addTagHelper *, Codehaks.Pagination` in `_ViewImports.cshtml`) and that Bootstrap CSS is
loaded. See [Getting started](getting-started.md) if not.

- [Razor Pages + EF Core (async)](#razor-pages--ef-core-async)
- [MVC controller + view](#mvc-controller--view)
- [One-call paging with `ToPagedResult`](#one-call-paging-with-topagedresult)
- [Paging an in-memory list](#paging-an-in-memory-list)
- [Projecting to a view model](#projecting-to-a-view-model)
- [Adjusting the window size](#adjusting-the-window-size)
- [Preserving filters in the URL](#preserving-filters-in-the-url)
- [Showing a row counter](#showing-a-row-counter)
- [Guarding bad page numbers](#guarding-bad-page-numbers)

---

## Razor Pages + EF Core (async)

The recommended shape for EF Core: `Page(...)` keeps the query composable so you can
materialize with `ToListAsync()` and never block a thread.

```csharp
// Users.cshtml.cs
public class UsersModel : PageModel
{
    private const int Size = 10;
    private readonly AppDbContext _db;
    public UsersModel(AppDbContext db) => _db = db;

    public IReadOnlyList<User> Users { get; private set; } = [];
    public int PageNumber { get; private set; }
    public int PageSize => Size;
    public int TotalPages { get; private set; }

    public async Task OnGetAsync(int number = 1, CancellationToken ct = default)
    {
        if (number < 1) number = 1;

        var total = await _db.Users.CountAsync(ct);
        Users = await _db.Users
            .OrderBy(u => u.Id)
            .Page(number, Size)
            .ToListAsync(ct);

        PageNumber = number;
        TotalPages = (total + Size - 1) / Size;
    }
}
```

```cshtml
@* Users.cshtml *@
@page "{number?}"
@model UsersModel

@foreach (var u in Model.Users) { <p>@u.Name</p> }

<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/users"
            page-range="5" />
```

---

## MVC controller + view

```csharp
public class UsersController : Controller
{
    private const int Size = 10;
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [HttpGet("/users/{number:int?}")]
    public async Task<IActionResult> Index(int number = 1, CancellationToken ct = default)
    {
        if (number < 1) number = 1;

        var total = await _db.Users.CountAsync(ct);
        var items = await _db.Users.OrderBy(u => u.Id).Page(number, Size).ToListAsync(ct);

        var vm = new PagedResult<User>(items, number, Size, total);
        return View(vm);
    }
}
```

```cshtml
@* Views/Users/Index.cshtml *@
@model Codehaks.Pagination.PagedResult<User>

@foreach (var u in Model.Items) { <p>@u.Name</p> }

<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/users"
            page-range="5" />
```

Note the route template `{number:int?}` — it's what makes `/users/3` map to `number = 3`.

---

## One-call paging with `ToPagedResult`

Simplest when you're **not** in an async-critical path (in-memory data, background jobs,
small admin screens). Remember it runs the count and page queries **synchronously**.

```csharp
PagedResult<Order> page = db.Orders
    .Where(o => o.Status == OrderStatus.Open)
    .OrderByDescending(o => o.CreatedAt)
    .ToPagedResult(pageNumber: 2, pageSize: 25);
```

```cshtml
@model PagedResult<Order>
<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/orders"
            page-range="5" />
```

---

## Paging an in-memory list

The helpers work on any `IQueryable<T>` — call `.AsQueryable()` on a list/array:

```csharp
string[] all = GetEverything();

PagedResult<string> page = all
    .AsQueryable()
    .OrderBy(x => x)
    .ToPagedResult(pageNumber: 1, pageSize: 20);
```

For LINQ-to-Objects there's no async provider, so `ToPagedResult` (sync) is the natural fit.

---

## Projecting to a view model

`Page` composes anywhere in the chain, so project **after** paging to keep the SQL tight —
only the page's rows are projected:

```csharp
var rows = await db.Users
    .Where(u => u.IsActive)
    .OrderBy(u => u.Id)
    .Page(number, Size)
    .Select(u => new UserRow(u.Id, u.Name, u.Email))   // SELECT only what the view needs
    .ToListAsync(ct);

var total = await db.Users.Where(u => u.IsActive).CountAsync(ct);  // same filter!
```

---

## Adjusting the window size

`page-range` controls how many numbered links show. The window always holds `2 × page-range`
links (clamped to `page-count`).

```cshtml
@* Compact: up to 6 numbered links *@
<pagination page-count="@Model.TotalPages" page-number="@Model.PageNumber"
            page-target="/users" page-range="3" />

@* Wide: up to 20 numbered links *@
<pagination page-count="@Model.TotalPages" page-number="@Model.PageNumber"
            page-target="/users" page-range="10" />
```

---

## Preserving filters in the URL

The Tag Helper builds **path-style** links (`/users/3`) and does not carry query-string
state. If you page a **filtered/sorted** list, the filter must survive page navigation.

**Option A — put filters in the route path** (works with the Tag Helper as-is):

```
/users/active/3      →  route: /users/{status}/{number:int?}, page-target="/users/active"
```

```csharp
[HttpGet("/users/{status}/{number:int?}")]
public IActionResult Index(string status, int number = 1) { /* … */ }
```

```cshtml
<pagination page-count="@Model.TotalPages" page-number="@Model.PageNumber"
            page-target="@($"/users/{Model.Status}")" page-range="5" />
```

**Option B — render your own links** when you truly need `?page=N&q=…` query strings. The
Tag Helper doesn't emit those today (a query-string mode is a candidate future feature), so
build the `<ul class="pagination">` yourself from `PagedResult<T>`:

```cshtml
@model PagedResult<User>
<nav aria-label="Page navigation">
  <ul class="pagination">
    @for (var p = 1; p <= Model.TotalPages; p++)
    {
      <li class="page-item @(p == Model.PageNumber ? "active" : "")">
        <a class="page-link" asp-route-page="@p" asp-route-q="@Context.Request.Query["q"]">@p</a>
      </li>
    }
  </ul>
</nav>
```

---

## Showing a row counter

Compute the running row number from the current page and size:

```cshtml
@{ var row = (Model.PageNumber - 1) * Model.PageSize + 1; }
@foreach (var u in Model.Items)
{
    <tr><td>@row</td><td>@u.Name</td></tr>
    row++;
}
```

---

## Guarding bad page numbers

`Page`/`ToPagedResult` throw `ArgumentOutOfRangeException` for `pageNumber < 1` or
`pageSize <= 0`, so coerce input first:

```csharp
if (number < 1) number = 1;
```

A page number **past** the last page does not throw — it returns an empty `Items` list. If
you'd rather redirect to the last valid page:

```csharp
var totalPages = (total + Size - 1) / Size;
if (totalPages > 0 && number > totalPages)
    return RedirectToPage(new { number = totalPages });
```
