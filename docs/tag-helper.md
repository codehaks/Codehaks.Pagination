# Tag Helper reference

The `<pagination>` Tag Helper renders a Bootstrap pagination control. Register it once in
`_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Codehaks.Pagination
```

Then use it in any view (the element name is case-insensitive — `<pagination>` and
`<Pagination>` both work):

```cshtml
<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/users"
            page-range="5" />
```

## Attributes

| Attribute       | Type   | Default | Description |
|-----------------|--------|---------|-------------|
| `page-count`    | int    | —       | Total number of pages. Drives the `Last` link and the upper clamp of the window. |
| `page-number`   | int    | —       | The current (1-based) page. Marked `active`; drives `Prev`/`Next` targets and `disabled` edges. |
| `page-target`   | string | —       | URL base each link points at. A link to page _n_ is `{page-target}/{n}` (e.g. `/users` → `/users/3`). |
| `page-range`    | int    | `5`     | How many links to show on each side of the current page (the window). See [Window sizing](#window-sizing). |
| `page-size`     | int    | `10`    | **Currently does not affect the rendered markup** — kept for API completeness. Page sizing happens server-side in the paging helpers, not here. |
| `page-first`    | string | `First` | Label for the first-page link. |
| `page-last`     | string | `Last`  | Label for the last-page link. |
| `page-previous` | string | `Previous` | Label for the previous-page link. |
| `page-next`     | string | `Next`  | Label for the next-page link. |

> `page-count`, `page-number`, and `page-target` are the three you must supply. The rest have
> sensible defaults.

## Markup it emits

For `page-number="3"`, `page-count="10"`, `page-range="2"`, `page-target="/users"`:

```html
<nav aria-label="Page navigation">
  <ul class="pagination">
    <li class="page-item"><a class="page-link" href="/users/1">First</a></li>
    <li class="page-item"><a class="page-link" href="/users/2">Previous</a></li>
    <li class="page-item"><a class="page-link" href="/users/1">1</a></li>
    <li class="page-item"><a class="page-link" href="/users/2">2</a></li>
    <li class="page-item active"><a class="page-link" aria-current="page" href="/users/3">3</a></li>
    <li class="page-item"><a class="page-link" href="/users/4">4</a></li>
    <li class="page-item"><a class="page-link" href="/users/5">5</a></li>
    <li class="page-item"><a class="page-link" href="/users/4">Next</a></li>
    <li class="page-item"><a class="page-link" href="/users/10">Last</a></li>
  </ul>
</nav>
```

Key points:

- The **active** page gets both `class="… active"` **and** `aria-current="page"` on its link,
  so screen readers announce it.
- **Disabled** edge links (see below) render as
  `<li class="page-item disabled"><a class="page-link" href="#" tabindex="-1" aria-disabled="true">…</a></li>`
  — a `#` href with `tabindex="-1"` so they aren't navigable or focusable.
- The outer element is a `<nav aria-label="Page navigation">`.

## Disabled edges

- On the **first** page (`page-number <= 1`), `First` and `Previous` are `disabled`.
- On the **last** page (`page-number >= page-count`), `Next` and `Last` are `disabled`.

This prevents links to page 0 or page `count + 1`.

## Window sizing

`page-range` controls how many numbered links appear. The window holds a **consistent**
`2 × page-range` links (clamped to `page-count`) no matter where the current page is — the
window slides as you move through the pages, keeping the current page in view.

- `page-range="5"` → up to 10 numbered links.
- If `page-count` is smaller than the window, all pages are shown.
- `page-range` defaults to `5`; a non-positive value falls back to `5`.

> There are currently **no `…` (ellipsis) gap indicators** between the window and the
> first/last pages — the `First`/`Last` links serve that role. Ellipsis support is a possible
> future enhancement.

## Routing

Links are **path-style**: a link to page _n_ is exactly `{page-target}/{n}`. To make those
URLs resolve, your endpoint must accept the page number as a **route segment**.

**Razor Pages:**

```cshtml
@page "{number?}"
```

```csharp
public async Task OnGetAsync(int number = 1) { /* … */ }
```

**MVC:**

```csharp
[HttpGet("/users/{number:int?}")]
public IActionResult Index(int number = 1) { /* … */ }
```

The Tag Helper does **not** emit query-string links (`/users?page=3`). If your app pages via
query string or needs to preserve other filters in the URL, see
[Preserving filters / query strings](examples.md#preserving-filters-in-the-url) for the
current workaround.

## Accessibility checklist

- ✅ `<nav aria-label="Page navigation">` wrapper
- ✅ `aria-current="page"` on the active link
- ✅ `aria-disabled="true"` + `tabindex="-1"` on disabled edge links
- ℹ️ Provide meaningful labels via `page-previous` / `page-next` etc. when localizing — see
  [Localization](localization.md).
