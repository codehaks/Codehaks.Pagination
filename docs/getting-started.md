# Getting started

This walks through paging a list and rendering the control end to end in an
ASP.NET Core Razor Pages app. The same building blocks work in MVC — see
[Examples by use case](examples.md).

## 1. Install

```sh
dotnet add package Codehaks.Pagination
```

## 2. Register the Tag Helper

Add the assembly to `_ViewImports.cshtml` so Razor recognizes the `<pagination>` element:

```cshtml
@addTagHelper *, Codehaks.Pagination
```

> One registration in `_ViewImports.cshtml` covers every view in that folder and below.

## 3. Make sure Bootstrap CSS is loaded

The control emits Bootstrap's `pagination` / `page-item` / `page-link` classes. It does
**not** ship any CSS — your layout must already include Bootstrap 4 or 5, e.g.:

```html
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
```

## 4. Page your data (the PageModel)

Pick **one** of the two helpers (full comparison in
[Paging helpers](paging-helpers.md)):

- `ToPagedResult(...)` — one call, returns items **and** all the metadata. Synchronous.
- `Page(...)` — just `Skip`/`Take`; you fetch the total count and compute pages yourself.
  Composes with `ToListAsync()` for async EF Core.

Here is the async approach (recommended with EF Core):

```csharp
using Codehaks.Pagination;

public class UsersModel : PageModel
{
    private const int PageSizeValue = 10;
    private readonly AppDbContext _db;

    public UsersModel(AppDbContext db) => _db = db;

    public IReadOnlyList<User> Users { get; private set; } = [];
    public int PageNumber { get; private set; }
    public int PageSize => PageSizeValue;
    public int TotalPages { get; private set; }

    public async Task OnGetAsync(int number = 1, CancellationToken ct = default)
    {
        if (number < 1) number = 1;          // guard: Page(...) throws for page < 1

        var totalCount = await _db.Users.CountAsync(ct);

        Users = await _db.Users
            .OrderBy(u => u.Id)              // ALWAYS order before paging
            .Page(number, PageSizeValue)     // Skip/Take only — still IQueryable
            .ToListAsync(ct);               // EF Core materializes asynchronously

        PageNumber = number;
        TotalPages = (totalCount + PageSizeValue - 1) / PageSizeValue;  // ceiling
    }
}
```

> **Always call `OrderBy` before paging.** `Skip`/`Take` over an unordered query has no
> stable meaning and EF Core will warn. Order by a unique, stable key (usually the PK).

## 5. Add a route segment for the page number

For path-style links like `/users/3` to work, the page must accept the page number as a
route value. In Razor Pages:

```cshtml
@page "{number?}"
@model UsersModel
```

The `{number?}` segment matches the `number` parameter on `OnGetAsync`. The `?` makes it
optional so `/users` (page 1) also works.

> The Tag Helper builds links as `{page-target}/{page}` (e.g. `/users/3`). It does **not**
> generate query-string links like `/users?page=3`. See [Routing](tag-helper.md#routing).

## 6. Render the control (the view)

```cshtml
@page "{number?}"
@model UsersModel

<table class="table">
    <thead><tr><th>#</th><th>Name</th></tr></thead>
    <tbody>
    @{ var row = (Model.PageNumber - 1) * Model.PageSize + 1; }
    @foreach (var user in Model.Users)
    {
        <tr><td>@row</td><td>@user.Name</td></tr>
        row++;
    }
    </tbody>
</table>

<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/users"
            page-range="5" />
```

That's it. You get `First`/`Previous`, a window of numbered links (the current one marked
`active` with `aria-current="page"`), then `Next`/`Last`, with edge links `disabled` on the
first/last page.

## Next steps

- [Paging helpers](paging-helpers.md) — sync vs async, `PagedResult<T>`, validation rules.
- [Tag Helper reference](tag-helper.md) — every attribute and the exact markup.
- [Examples by use case](examples.md) — MVC, in-memory lists, filters, and more.
