# Paging helpers

All paging helpers live in the `Codehaks.Pagination` namespace:

```csharp
using Codehaks.Pagination;
```

There are two extension methods on `IQueryable<T>` and one metadata record.

| Member | Returns | Executes a query? |
|--------|---------|-------------------|
| `Page(pageNumber, pageSize)` | `IQueryable<T>` | No — adds `Skip`/`Take`, stays composable. |
| `ToPagedResult(pageNumber, pageSize)` | `PagedResult<T>` | Yes — runs a count query **and** a page query (synchronously). |
| `PagedResult<T>` | record | — holds items + metadata. |

---

## `Page<T>` — Skip/Take, nothing else

```csharp
public static IQueryable<T> Page<T>(this IQueryable<T> source, int pageNumber, int pageSize)
```

Adds `Skip(pageSize * (pageNumber - 1)).Take(pageSize)` and returns the query **unmaterialized**.
Because it stays `IQueryable<T>`, it composes with the rest of your LINQ pipeline and with
async materialization:

```csharp
// Synchronous (LINQ to Objects, or blocking EF Core):
List<User> page = db.Users.OrderBy(u => u.Id).Page(2, 10).ToList();

// Asynchronous (EF Core) — this is why Page returns IQueryable:
List<User> page = await db.Users.OrderBy(u => u.Id).Page(2, 10).ToListAsync(ct);

// Project before materializing — Page composes anywhere in the chain:
var rows = await db.Users
    .Where(u => u.IsActive)
    .OrderBy(u => u.Id)
    .Page(2, 10)
    .Select(u => new UserRow(u.Id, u.Name))
    .ToListAsync(ct);
```

With `Page`, **you** are responsible for the total count and page math:

```csharp
var total = await db.Users.Where(u => u.IsActive).CountAsync(ct);
var totalPages = (total + pageSize - 1) / pageSize;   // ceiling division
```

> Apply the same filters to your count query as to your page query, or the page count
> won't match the data.

---

## `ToPagedResult<T>` — one call, items + metadata

```csharp
public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> source, int pageNumber, int pageSize)
```

Convenience method: issues `source.Count()` and `source.Page(...).ToList()`, then packs both
into a `PagedResult<T>`.

```csharp
PagedResult<User> result = db.Users
    .Where(u => u.IsActive)
    .OrderBy(u => u.Id)
    .ToPagedResult(pageNumber: 2, pageSize: 10);

result.Items;            // IReadOnlyList<User> — the 10 users on page 2
result.TotalCount;       // total matching rows (before paging)
result.TotalPages;       // ceiling(TotalCount / PageSize)
result.HasPreviousPage;  // PageNumber > 1
result.HasNextPage;      // PageNumber < TotalPages
```

### `ToPagedResult` is synchronous

It calls `.Count()` and `.ToList()`, which **block** on an EF Core provider. There is no
`ToPagedResultAsync`. Choose based on your context:

- **In-memory data, small sets, or non-async stacks** → `ToPagedResult` is simplest.
- **EF Core in an async request path** → prefer `Page(...)` + `ToListAsync()` + a
  `CountAsync()`, so you never block a thread:

  ```csharp
  var total = await query.CountAsync(ct);
  var items = await query.Page(n, size).ToListAsync(ct);
  var result = new PagedResult<User>(items, n, size, total);  // build the record yourself
  ```

  This gives you the exact same `PagedResult<T>` (with all its computed metadata) without a
  blocking call.

---

## `PagedResult<T>` — the metadata record

```csharp
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    public int TotalPages    => PageSize <= 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage     => PageNumber < TotalPages;
}
```

- It's a positional `record`, so it has value equality and a `Deconstruct`.
- `TotalPages` is ceiling division; it returns `0` for a non-positive `PageSize` (defensive).
- `Items` is `IReadOnlyList<T>` — iterate or index it; it won't be re-queried.
- You can construct it directly (see the async pattern above) — the metadata properties are
  computed from the four values you pass.

These three properties map straight onto the Tag Helper:

```cshtml
<pagination page-count="@result.TotalPages"
            page-number="@result.PageNumber"
            page-target="/users"
            page-range="5" />
```

---

## Validation & exceptions

Both `Page` and `ToPagedResult` validate their arguments up front:

| Condition | Exception |
|-----------|-----------|
| `source` is `null` | `ArgumentNullException` |
| `pageNumber < 1` | `ArgumentOutOfRangeException` |
| `pageSize <= 0` | `ArgumentOutOfRangeException` |

So clamp user input **before** calling them — a typical controller coerces `?number=0` or a
missing value to `1`:

```csharp
if (number < 1) number = 1;
```

Page numbers **beyond** the last page do not throw — they simply return an empty `Items`
list (`Skip` runs off the end). Decide in your UI whether to redirect such requests back to
the last valid page.
