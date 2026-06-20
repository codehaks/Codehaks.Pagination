# Troubleshooting

Common issues and what they mean.

## The `<pagination>` element renders as literal HTML / isn't processed

The Tag Helper isn't registered. Add it to `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Codehaks.Pagination
```

`_ViewImports.cshtml` applies to its folder and below — put it at `Pages/` (Razor Pages) or
`Views/` (MVC) level, or in a subfolder to scope it.

## Clicking a page link gives a 404

The links are **path-style** (`/users/3`). Your endpoint must accept the page number as a
**route segment**:

- Razor Pages: `@page "{number?}"` and an `OnGet(int number = 1)` parameter.
- MVC: a route like `[HttpGet("/users/{number:int?}")]` with an `int number = 1` parameter.

Also check `page-target` matches the path: `page-target="/users"` produces `/users/3`. A
mismatched or missing leading slash is the usual culprit.

> The Tag Helper does **not** generate `?page=3` query-string links. If that's what you need,
> see [Preserving filters in the URL](examples.md#preserving-filters-in-the-url).

## The control has no styling (plain bullet list / links)

Bootstrap CSS isn't loaded. The library emits Bootstrap classes (`pagination`, `page-item`,
`page-link`) but ships no CSS. Add Bootstrap 4 or 5 to your layout.

## Labels show as `&#1602;&#1576;…` in the page source

Expected default encoder behavior for non-Latin text — it still displays correctly in the
browser. To emit literal characters, widen `WebEncoderOptions`. See
[Localization → Non-Latin labels](localization.md#non-latin-labels-rendering-as-numeric-entities).

## `ArgumentOutOfRangeException` from `Page` / `ToPagedResult`

Thrown when `pageNumber < 1` or `pageSize <= 0`. Coerce user input before calling:

```csharp
if (number < 1) number = 1;
```

## `ArgumentNullException` from `Page` / `ToPagedResult`

The `source` query is `null`. Make sure the `IQueryable<T>` (e.g. your `DbSet`) is actually
initialized.

## EF Core warns about `Skip`/`Take` without `OrderBy`

Always `OrderBy` a stable, unique key **before** `Page(...)`. Without a defined order,
`Skip`/`Take` can return overlapping or missing rows between pages.

## A high page number shows an empty page instead of erroring

By design. Requesting a page past the last one returns an empty `Items` list (it doesn't
throw). Redirect to the last valid page if you'd rather:

```csharp
var totalPages = (total + size - 1) / size;
if (totalPages > 0 && number > totalPages)
    return RedirectToPage(new { number = totalPages });
```

## `ToPagedResult` blocks / I want async

`ToPagedResult` is synchronous (it calls `.Count()` and `.ToList()`). There is no async
overload. In async EF Core paths use `Page(...)` + `ToListAsync()` + `CountAsync()` and build
the `PagedResult<T>` yourself — see
[Paging helpers → ToPagedResult is synchronous](paging-helpers.md#topagedresult-is-synchronous).

## The page window doesn't show a `…` ellipsis

There is no ellipsis indicator currently — the `First`/`Last` links provide the jump-to-edge
behavior instead. Ellipsis support is a possible future enhancement.
