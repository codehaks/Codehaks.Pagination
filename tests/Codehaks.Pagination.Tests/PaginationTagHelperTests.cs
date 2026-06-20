using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Codehaks.Pagination;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace Codehaks.Pagination.Tests;

public class PaginationTagHelperTests
{
    private static string Render(PaginationTagHelper helper)
    {
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test");

        var output = new TagHelperOutput(
            "pagination",
            new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        helper.Process(context, output);

        using var writer = new StringWriter();
        // Allow all Unicode through so non-Latin labels are assertable (mirrors an app that
        // configures WebEncoderOptions for non-Latin languages); the default encoder would
        // otherwise escape them to numeric entities.
        output.Content.WriteTo(writer, HtmlEncoder.Create(UnicodeRanges.All));
        return writer.ToString();
    }

    private static PaginationTagHelper Build(int pageNumber, int pageCount) => new()
    {
        PageNumber = pageNumber,
        PageCount = pageCount,
        PageRange = 5,
        PageTarget = "/index",
    };

    [Fact]
    public void Renders_a_properly_closed_unordered_list()
    {
        var html = Render(Build(pageNumber: 1, pageCount: 10));

        // Regression: the old code emitted "</ul" without the closing ">".
        Assert.Contains("<ul class=\"pagination\">", html, System.StringComparison.Ordinal);
        Assert.EndsWith("</ul>", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Renders_anchor_with_a_space_before_href()
    {
        var html = Render(Build(pageNumber: 1, pageCount: 10));

        // Regression: the old code emitted class='page-link'href=... with no space.
        Assert.Contains("<a class=\"page-link\" href=", html, System.StringComparison.Ordinal);
        Assert.DoesNotContain("page-link\"href", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Marks_the_current_page_active()
    {
        var html = Render(Build(pageNumber: 3, pageCount: 10));

        Assert.Contains("<li class=\"page-item active\">", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Builds_hrefs_from_the_page_target()
    {
        var html = Render(Build(pageNumber: 1, pageCount: 10));

        Assert.Contains("href=\"/index/1\"", html, System.StringComparison.Ordinal);
        Assert.Contains("href=\"/index/10\"", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Uses_default_first_and_last_labels()
    {
        var html = Render(Build(pageNumber: 1, pageCount: 10));

        Assert.Contains(">First</a>", html, System.StringComparison.Ordinal);
        Assert.Contains(">Last</a>", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Renders_default_previous_and_next_labels()
    {
        var html = Render(Build(pageNumber: 5, pageCount: 10));

        Assert.Contains(">Previous</a>", html, System.StringComparison.Ordinal);
        Assert.Contains(">Next</a>", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Previous_links_to_the_prior_page()
    {
        var html = Render(Build(pageNumber: 5, pageCount: 10));

        // Previous → page 4, Next → page 6.
        Assert.Contains("href=\"/index/4\"", html, System.StringComparison.Ordinal);
        Assert.Contains("href=\"/index/6\"", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void First_and_previous_are_disabled_on_the_first_page()
    {
        var html = Render(Build(pageNumber: 1, pageCount: 10));

        // First + Previous disabled; Next + Last active.
        Assert.Contains("<li class=\"page-item disabled\">", html, System.StringComparison.Ordinal);
        Assert.Contains("aria-disabled=\"true\"", html, System.StringComparison.Ordinal);
        Assert.DoesNotContain("href=\"/index/0\"", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Next_and_last_are_disabled_on_the_last_page()
    {
        var html = Render(Build(pageNumber: 10, pageCount: 10));

        Assert.Contains("<li class=\"page-item disabled\">", html, System.StringComparison.Ordinal);
        // Next would be page 11 — must not be linkable.
        Assert.DoesNotContain("href=\"/index/11\"", html, System.StringComparison.Ordinal);
    }

    [Fact]
    public void Custom_previous_and_next_labels_are_used()
    {
        var helper = Build(pageNumber: 5, pageCount: 10);
        helper.PagePrevious = "قبلی";
        helper.PageNext = "بعدی";

        var html = Render(helper);

        Assert.Contains("قبلی", html, System.StringComparison.Ordinal);
        Assert.Contains("بعدی", html, System.StringComparison.Ordinal);
    }
}
