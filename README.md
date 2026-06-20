# Codehaks.Pagination

A Bootstrap-styled pagination **Tag Helper** and lightweight `IQueryable<T>` paging helpers
for ASP.NET Core.

![Pagination control](pagination.JPG)

## What you get

- **`Page(...)`** — adds `Skip`/`Take` to any `IQueryable<T>`, staying composable (works with
  `ToListAsync()` for async EF Core).
- **`ToPagedResult(...)`** — one synchronous call that returns the page's items plus
  `TotalCount` / `TotalPages` / `HasNextPage` / `HasPreviousPage`.
- **`<pagination>` Tag Helper** — renders a Bootstrap `<nav><ul class="pagination">…</ul></nav>`
  with First/Previous, a sliding window of numbered links (active one marked with
  `aria-current="page"`), and Next/Last, with `disabled` edges.

## Requirements

- .NET 10 / ASP.NET Core
- Bootstrap 4+ CSS loaded on the page (the library emits the classes but ships no CSS)

## Install

```sh
dotnet add package Codehaks.Pagination
```

Register the Tag Helper in `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Codehaks.Pagination
```

## Quick start

**Page your data** (Razor Pages, async EF Core):

```csharp
using Codehaks.Pagination;

public async Task OnGetAsync(int number = 1, CancellationToken ct = default)
{
    if (number < 1) number = 1;

    var total = await _db.Users.CountAsync(ct);
    Users = await _db.Users
        .OrderBy(u => u.Id)            // always order before paging
        .Page(number, PageSize)
        .ToListAsync(ct);

    PageNumber = number;
    TotalPages = (total + PageSize - 1) / PageSize;
}
```

**Render the control** (links are path-style, so add a route segment):

```cshtml
@page "{number?}"

<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/users"
            page-range="5" />
```

That produces `/users/1`, `/users/2`, … links. Prefer one call? Use
`ToPagedResult(...)` (synchronous) instead — see the docs.

## Documentation

Full guides with examples for every use case live in [`docs/`](docs/README.md):

| Guide | What it covers |
|-------|----------------|
| [Getting started](docs/getting-started.md) | Install → page data → render, end to end. |
| [Paging helpers](docs/paging-helpers.md) | `Page`, `ToPagedResult`, `PagedResult<T>`, sync vs. async, validation. |
| [Tag Helper reference](docs/tag-helper.md) | Every attribute, the exact markup, routing, accessibility. |
| [Examples by use case](docs/examples.md) | Razor Pages, MVC, in-memory lists, projections, preserving filters. |
| [Localization & custom labels](docs/localization.md) | Custom labels, RTL, non-Latin text encoding. |
| [Troubleshooting](docs/troubleshooting.md) | 404s on links, unstyled control, encoding, and more. |

## Sample project

A runnable Razor Pages sample lives in
[`src/Codehaks.Pagination.Sample`](src/Codehaks.Pagination.Sample). It uses a throwaway SQLite
database created and seeded on first run, so you can just:

```sh
dotnet run --project src/Codehaks.Pagination.Sample
```

## Building & testing

```sh
dotnet build
dotnet test
```

## Bugs & issues

Please report bugs on the [issue tracker](https://github.com/Codehaks/Pagination/issues).

## License

MIT
