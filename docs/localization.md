# Localization & custom labels

The four edge links — `First`, `Previous`, `Next`, `Last` — have customizable labels. Set
them per-control or wire them to your localization system.

## Custom labels

```cshtml
<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/users"
            page-range="5"
            page-first="««"
            page-previous="‹"
            page-next="›"
            page-last="»»" />
```

Defaults when an attribute is omitted (or empty): `First`, `Previous`, `Next`, `Last`.

The numbered links always show the page number itself — only the four edge labels are
configurable.

## Localized labels (IStringLocalizer)

Feed the labels from your resource files:

```cshtml
@inject IStringLocalizer<SharedResource> L

<pagination page-count="@Model.TotalPages"
            page-number="@Model.PageNumber"
            page-target="/users"
            page-range="5"
            page-first="@L["First"]"
            page-previous="@L["Previous"]"
            page-next="@L["Next"]"
            page-last="@L["Last"]" />
```

## Right-to-left (RTL) languages

The control is plain Bootstrap markup, so RTL works the way it does everywhere else — set the
document/container direction and let Bootstrap mirror it:

```html
<html dir="rtl" lang="fa">
```

or scope it:

```html
<nav dir="rtl">
  <pagination page-count="@Model.TotalPages" page-number="@Model.PageNumber"
              page-target="/users" page-range="5"
              page-first="نخست" page-previous="قبلی"
              page-next="بعدی" page-last="آخر" />
</nav>
```

## Non-Latin labels rendering as numeric entities

By default, ASP.NET Core's HTML encoder escapes characters outside Basic Latin to **numeric
HTML entities** (e.g. `قبلی` → `&#1602;&#1576;&#1604;&#1740;`). Browsers still display them
correctly, but the page source is unreadable and some tooling trips on it.

To emit the characters literally, widen the web encoder in `Program.cs`:

```csharp
using System.Text.Encodings.Web;
using System.Text.Unicode;

builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
```

Narrow it to just the ranges you need (smaller attack surface) if you prefer, e.g.
`UnicodeRanges.BasicLatin, UnicodeRanges.Arabic`.

> This is a global ASP.NET Core behavior, not specific to this library — it affects all Razor
> output. The library's own tests configure the encoder this way so non-Latin labels are
> assertable.
