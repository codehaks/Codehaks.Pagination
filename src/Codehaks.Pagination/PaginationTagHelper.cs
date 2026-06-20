using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Codehaks.Pagination;

/// <summary>
/// Renders a Bootstrap-styled pagination control as
/// <c>&lt;nav&gt;&lt;ul class="pagination"&gt;…&lt;/ul&gt;&lt;/nav&gt;</c>.
/// Usage in Razor: <c>&lt;pagination page-count="@Model.TotalPages" page-target="/index"
/// page-number="@Model.PageNumber" page-range="5"&gt;&lt;/pagination&gt;</c>.
/// </summary>
public class PaginationTagHelper : TagHelper
{
    /// <summary>The current (1-based) page number.</summary>
    public int PageNumber { get; set; }

    /// <summary>The number of items per page. Defaults to 10 when not positive.</summary>
    public int PageSize { get; set; }

    /// <summary>The total number of pages.</summary>
    public int PageCount { get; set; }

    /// <summary>How many page links to show on each side of the current page. Defaults to 5.</summary>
    public int PageRange { get; set; }

    /// <summary>Label for the link to the first page. Defaults to "First".</summary>
    public string? PageFirst { get; set; }

    /// <summary>Label for the link to the last page. Defaults to "Last".</summary>
    public string? PageLast { get; set; }

    /// <summary>Label for the link to the previous page. Defaults to "Previous".</summary>
    public string? PagePrevious { get; set; }

    /// <summary>Label for the link to the next page. Defaults to "Next".</summary>
    public string? PageNext { get; set; }

    /// <summary>The URL base that each page link points at, e.g. <c>/index</c>.</summary>
    public string? PageTarget { get; set; }

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        InitDefaults();

        output.TagName = "nav";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("aria-label", "Page navigation");

        var list = new TagBuilder("ul");
        list.AddCssClass("pagination");

        list.InnerHtml.AppendHtml(BuildPageItem(1, PageFirst!, isDisabled: PageNumber <= 1));
        list.InnerHtml.AppendHtml(BuildPageItem(PageNumber - 1, PagePrevious!, isDisabled: PageNumber <= 1));

        var (start, end) = ComputeWindow();
        for (var page = start; page <= end; page++)
        {
            list.InnerHtml.AppendHtml(BuildPageItem(page, page.ToString(CultureInfo.InvariantCulture), isActive: page == PageNumber));
        }

        list.InnerHtml.AppendHtml(BuildPageItem(PageNumber + 1, PageNext!, isDisabled: PageNumber >= PageCount));
        list.InnerHtml.AppendHtml(BuildPageItem(PageCount, PageLast!, isDisabled: PageNumber >= PageCount));

        output.Content.SetHtmlContent(list);
    }

    private (int start, int end) ComputeWindow()
    {
        int start, end;

        if (PageNumber <= PageRange)
        {
            start = 1;
            end = 2 * PageRange;
        }
        else if (PageNumber < PageCount - PageRange)
        {
            start = PageNumber - PageRange;
            end = PageNumber + PageRange - 1;
        }
        else
        {
            start = PageCount - (2 * PageRange);
            end = PageCount;
        }

        // Clamp to the valid [1, PageCount] range.
        return (Math.Max(start, 1), Math.Min(end, PageCount));
    }

    private TagBuilder BuildPageItem(int page, string text, bool isActive = false, bool isDisabled = false)
    {
        var item = new TagBuilder("li");
        item.AddCssClass("page-item");
        if (isActive)
        {
            item.AddCssClass("active");
        }

        if (isDisabled)
        {
            item.AddCssClass("disabled");
        }

        var link = new TagBuilder("a");
        link.AddCssClass("page-link");
        if (isDisabled)
        {
            // No navigation target on a disabled control; keep it focusable-inert.
            link.Attributes["href"] = "#";
            link.Attributes["tabindex"] = "-1";
            link.Attributes["aria-disabled"] = "true";
        }
        else
        {
            link.Attributes["href"] = FormattableString.Invariant($"{PageTarget}/{page}");
        }

        link.InnerHtml.Append(text);

        item.InnerHtml.AppendHtml(link);
        return item;
    }

    private void InitDefaults()
    {
        if (PageRange <= 0)
        {
            PageRange = 5;
        }

        if (PageCount < PageRange)
        {
            PageRange = PageCount;
        }

        if (PageSize <= 0)
        {
            PageSize = 10;
        }

        if (string.IsNullOrEmpty(PageFirst))
        {
            PageFirst = "First";
        }

        if (string.IsNullOrEmpty(PageLast))
        {
            PageLast = "Last";
        }

        if (string.IsNullOrEmpty(PagePrevious))
        {
            PagePrevious = "Previous";
        }

        if (string.IsNullOrEmpty(PageNext))
        {
            PageNext = "Next";
        }
    }
}
