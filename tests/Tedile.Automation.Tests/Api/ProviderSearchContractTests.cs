using System.Text.Json;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Api;

public sealed class ProviderSearchContractTests : BaseTest, IClassFixture<PlaywrightFixture>
{
    public ProviderSearchContractTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task Provider_search_returns_consistent_paging_metadata()
    {
        var response = await Page.GotoAsync("/api/search/providers?limit=1&offset=0&sort=rating-high");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;

        Assert.True(root.GetProperty("status").GetBoolean());
        Assert.Equal(1, root.GetProperty("limit").GetInt32());
        Assert.Equal(0, root.GetProperty("offset").GetInt32());

        var providers = root.GetProperty("data").GetProperty("providers");
        Assert.Equal(JsonValueKind.Array, providers.ValueKind);
        Assert.Equal(providers.GetArrayLength(), root.GetProperty("count").GetInt32());

        var total = root.GetProperty("total").GetInt32();
        Assert.True(total >= providers.GetArrayLength());

        if (total > 1)
        {
            Assert.Equal(1, root.GetProperty("next_offset").GetInt32());
        }
        else
        {
            Assert.Equal(JsonValueKind.Null, root.GetProperty("next_offset").ValueKind);
        }
    }

    [Fact]
    public async Task Consecutive_provider_search_pages_do_not_repeat_same_provider()
    {
        var first = await FetchPageAsync(0);

        if (first.Total < 2 || first.ProviderIds.Count == 0)
        {
            return;
        }

        var second = await FetchPageAsync(1);

        Assert.DoesNotContain(second.ProviderIds[0], first.ProviderIds);
    }

    [Fact]
    public async Task Verified_only_search_never_returns_unverified_provider()
    {
        var response = await Page.GotoAsync("/api/search/providers?verified_only=true&limit=50&offset=0");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        var providers = json.RootElement.GetProperty("data").GetProperty("providers");

        foreach (var provider in providers.EnumerateArray())
        {
            Assert.True(provider.GetProperty("verified").GetBoolean());
        }
    }

    [Fact]
    public async Task Active_services_endpoint_returns_unique_service_slugs()
    {
        var response = await Page.GotoAsync("/api/services");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        var services = json.RootElement.GetProperty("data");

        Assert.Equal(JsonValueKind.Array, services.ValueKind);
        Assert.True(services.GetArrayLength() > 0);

        var slugs = services.EnumerateArray()
            .Select(service => service.GetProperty("slug").GetString())
            .Where(slug => !string.IsNullOrWhiteSpace(slug))
            .ToList();

        Assert.Equal(slugs.Count, slugs.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    private async Task<(int Total, List<string> ProviderIds)> FetchPageAsync(int offset)
    {
        var response = await Page.GotoAsync($"/api/search/providers?limit=1&offset={offset}&sort=rating-high");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        var providers = root.GetProperty("data").GetProperty("providers");

        var ids = new List<string>();
        foreach (var provider in providers.EnumerateArray())
        {
            if (!provider.TryGetProperty("id", out var id))
            {
                continue;
            }

            ids.Add(id.ValueKind == JsonValueKind.String
                ? id.GetString() ?? string.Empty
                : id.ToString());
        }

        return (root.GetProperty("total").GetInt32(), ids);
    }
}
