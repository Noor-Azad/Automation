using System.Text.Json;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class SearchApiRobustnessTests : BaseTest
{
    [Test]
    public async Task Provider_search_sql_like_keyword_is_safely_handled_or_blocked_at_edge()
    {
        var response = await Page.GotoAsync("/api/search/providers?keyword=%27%20OR%201%3D1--&limit=10&offset=0");
        Assert.NotNull(response);

        var body = await Page.Locator("body").InnerTextAsync();

        if (response!.Status == 403)
        {
            // Production edge/WAF may reject obvious injection signatures before
            // the request reaches Tedile. That is an acceptable security outcome.
            Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("sqlalchemy", body, StringComparison.OrdinalIgnoreCase);
            return;
        }

        Assert.Equal(200, response.Status);
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("status").GetBoolean());
        Assert.True(json.RootElement.GetProperty("data").TryGetProperty("providers", out var providers));
        Assert.Equal(JsonValueKind.Array, providers.ValueKind);
    }

    [Test]
    public async Task Provider_search_xss_like_keyword_is_safely_handled_or_blocked_at_edge()
    {
        var response = await Page.GotoAsync("/api/search/providers?keyword=%3Cscript%3Ealert(1)%3C%2Fscript%3E&limit=10&offset=0");
        Assert.NotNull(response);

        var body = await Page.Locator("body").InnerTextAsync();

        if (response!.Status == 403)
        {
            // The production edge may block obvious script payload signatures.
            Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
            return;
        }

        Assert.Equal(200, response.Status);
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("status").GetBoolean());
        Assert.DoesNotContain("<script>", body, StringComparison.OrdinalIgnoreCase);
    }

        [TestCase("limit=0")]
    [TestCase("limit=51")]
    [TestCase("limit=abc")]
    [TestCase("offset=-1")]
    [TestCase("offset=abc")]
    [TestCase("radius=-1")]
    [TestCase("min_price=100&max_price=10")]
    [TestCase("latitude=91&longitude=88")]
    [TestCase("latitude=25")]
    public async Task Provider_search_rejects_invalid_query_boundaries(string query)
    {
        var response = await Page.GotoAsync($"/api/search/providers?{query}");
        Assert.NotNull(response);
        Assert.Equal(400, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.TryGetProperty("error", out _));
    }

    [Test]
    public async Task Provider_search_rejects_non_finite_coordinates()
    {
        var response = await Page.GotoAsync("/api/search/providers?latitude=NaN&longitude=88");
        Assert.NotNull(response);
        Assert.Equal(400, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.TryGetProperty("error", out _));
    }
}
