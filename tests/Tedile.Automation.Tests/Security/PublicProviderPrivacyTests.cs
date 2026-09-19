using System.Text.Json;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Security;

public sealed class PublicProviderPrivacyTests : BaseTest, IClassFixture<PlaywrightFixture>
{
    public PublicProviderPrivacyTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task Public_provider_search_does_not_expose_private_contact_or_exact_location_fields()
    {
        var response = await Page.GotoAsync("/api/search/providers?limit=50&offset=0&sort=rating-high");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        var providers = json.RootElement.GetProperty("data").GetProperty("providers");

        foreach (var provider in providers.EnumerateArray())
        {
            AssertPrivateFieldsAbsent(provider);
            Assert.True(provider.TryGetProperty("id", out var id));
            Assert.False(string.IsNullOrWhiteSpace(id.GetString()));
        }
    }

    [Fact]
    public async Task Public_provider_profile_does_not_expose_private_contact_or_exact_location_fields()
    {
        var searchResponse = await Page.GotoAsync("/api/search/providers?limit=1&offset=0&sort=rating-high");
        Assert.NotNull(searchResponse);
        Assert.Equal(200, searchResponse!.Status);

        var searchBody = await Page.Locator("body").InnerTextAsync();
        using var searchJson = JsonDocument.Parse(searchBody);
        var providers = searchJson.RootElement.GetProperty("data").GetProperty("providers");

        if (providers.GetArrayLength() == 0)
        {
            return;
        }

        var profileCode = providers[0].GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(profileCode));

        var profileResponse = await Page.GotoAsync($"/api/providers/{Uri.EscapeDataString(profileCode!)}");
        Assert.NotNull(profileResponse);
        Assert.Equal(200, profileResponse!.Status);

        var profileBody = await Page.Locator("body").InnerTextAsync();
        using var profileJson = JsonDocument.Parse(profileBody);
        var profile = profileJson.RootElement;

        AssertPrivateFieldsAbsent(profile);
        Assert.True(profile.TryGetProperty("services", out var services));
        Assert.Equal(JsonValueKind.Array, services.ValueKind);
        Assert.True(profile.TryGetProperty("recent_reviews", out var reviews));
        Assert.Equal(JsonValueKind.Array, reviews.ValueKind);
    }

    [Fact]
    public async Task Unknown_public_provider_profile_returns_not_found_without_internal_details()
    {
        var response = await Page.GotoAsync("/api/providers/AUTOMATION-NOT-A-REAL-PROVIDER");
        Assert.NotNull(response);
        Assert.Equal(404, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);

        Assert.Equal("Provider not found", json.RootElement.GetProperty("error").GetString());
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sqlalchemy", body, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertPrivateFieldsAbsent(JsonElement provider)
    {
        var forbidden = new[]
        {
            "phone",
            "whatsapp",
            "email",
            "user_id",
            "password",
            "password_hash",
            "latitude",
            "longitude",
            "registered_address",
            "pincode"
        };

        foreach (var field in forbidden)
        {
            Assert.False(
                provider.TryGetProperty(field, out _),
                $"Public provider payload unexpectedly exposed '{field}'.");
        }
    }
}
