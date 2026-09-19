using Microsoft.Playwright;

namespace Tedile.Automation.Tests.Security;

public sealed class AnonymousMutationAuthorizationTests : BaseTest
{
    [TestCase("/customer/bookings", "POST")]
    [TestCase("/provider/availability", "POST")]
    [TestCase("/provider/working-hours", "PUT")]
    [TestCase("/api/admin/providers", "POST")]
    public async Task Anonymous_mutation_requests_are_redirected_before_application_mutation(string path, string method)
    {
        await Fixture.InitializeAsync();
        await using var request = await Fixture.Playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions
            {
                BaseURL = Fixture.Settings.BaseUrl,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            });

        var response = await request.FetchAsync(
            path,
            new APIRequestContextOptions
            {
                Method = method,
                DataObject = new
                {
                    provider_profile_code = "AUTOMATION-NOT-A-REAL-PROVIDER",
                    availability = "available",
                    name = "Automation Should Not Create"
                },
                MaxRedirects = 0
            });

        Assert.Equal(302, response.Status);

        Assert.True(response.Headers.TryGetValue("location", out var location));
        Assert.True(
            location is "/" or "/login" || location.EndsWith("/", StringComparison.Ordinal),
            $"Expected anonymous mutation to redirect to login/welcome, but Location was '{location}'.");

        var body = await response.TextAsync();
        Assert.DoesNotContain("created", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("updated", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("saved", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
