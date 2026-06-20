using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
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
        output.Content.WriteTo(writer, HtmlEncoder.Default);
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
}
