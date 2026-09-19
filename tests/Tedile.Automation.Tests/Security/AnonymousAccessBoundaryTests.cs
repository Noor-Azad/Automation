using Microsoft.Playwright;

namespace Tedile.Automation.Tests.Security;

public sealed class AnonymousAccessBoundaryTests : BaseTest
{
    [TestCase("/customer/profile")]
    [TestCase("/notifications")]
    [TestCase("/customer/providers/AUTOMATION-NOT-A-REAL-PROVIDER/contact")]
    [TestCase("/customer/providers/AUTOMATION-NOT-A-REAL-PROVIDER/directions")]
    [TestCase("/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/tracking")]
    public async Task Anonymous_user_is_redirected_before_protected_customer_data_is_exposed(string path)
    {
        await Fixture.InitializeAsync();
        await using var request = await Fixture.Playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions
            {
                BaseURL = Fixture.Settings.BaseUrl,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept"] = "application/json"
                }
            });

        var response = await request.GetAsync(
            path,
            new APIRequestContextOptions
            {
                MaxRedirects = 0
            });

        Assert.Equal(302, response.Status);

        var headers = response.Headers;
        Assert.True(headers.TryGetValue("location", out var location));
        Assert.True(
            location is "/" or "/login" || location.EndsWith("/", StringComparison.Ordinal),
            $"Expected anonymous access to redirect to Tedile login/welcome, but Location was '{location}'.");

        var body = await response.TextAsync();
        Assert.DoesNotContain("phone", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("whatsapp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("latitude", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("longitude", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("otp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
