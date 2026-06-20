# Codehaks.Pagination — Documentation

A Bootstrap-styled pagination **Tag Helper** plus lightweight `IQueryable<T>` paging
helpers for ASP.NET Core.

The library has two halves you can use together or independently:

1. **Server-side paging helpers** — `Page(...)`, `ToPagedResult(...)`, and the
   `PagedResult<T>` metadata record. They slice an `IQueryable<T>` and (optionally) hand
   you the counts a UI needs.
2. **A pagination Tag Helper** — renders a Bootstrap `<nav><ul class="pagination">…</ul></nav>`
   control from that metadata.

## Contents

| Guide | What it covers |
|-------|----------------|
| [Getting started](getting-started.md) | Install, register the tag helper, render your first paged page end to end. |
| [Paging helpers](paging-helpers.md) | `Page`, `ToPagedResult`, `PagedResult<T>`, sync vs. async, validation. |
| [Tag Helper reference](tag-helper.md) | Every attribute, the markup it emits, routing rules, accessibility. |
| [Examples by use case](examples.md) | Razor Pages, MVC, async EF Core, in-memory lists, preserving filters, and more. |
| [Localization & custom labels](localization.md) | Custom First/Prev/Next/Last labels, RTL, non-Latin text encoding. |
| [Troubleshooting](troubleshooting.md) | Links 404, labels show as `&#…;`, control not rendering, and other gotchas. |

## Requirements

- .NET 10 / ASP.NET Core
- Bootstrap 4+ CSS loaded on the page (for `pagination` / `page-item` / `page-link` styles)

## Quick taste

```csharp
// In a page/controller — get one page plus metadata:
PagedResult<User> page = db.Users.OrderBy(u => u.Id).ToPagedResult(pageNumber: 2, pageSize: 10);
```

```cshtml
@* In the view — render the control: *@
<pagination page-count="@page.TotalPages"
            page-number="@page.PageNumber"
            page-target="/users"
            page-range="5" />
```

See [Getting started](getting-started.md) for the full walkthrough.
