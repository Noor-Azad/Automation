using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class AdminProviderAuthorizationMatrixTests : AuthenticatedCustomerBaseTest
{
    private const string FakeProvider = "AUTOMATION-NOT-A-REAL-PROVIDER";

    [TestCase($"/api/admin/providers/{FakeProvider}/allow-location-update")]
    [TestCase($"/api/admin/providers/{FakeProvider}/link-account")]
    [TestCase($"/api/admin/providers/{FakeProvider}/block")]
    [TestCase($"/api/admin/providers/{FakeProvider}/unblock")]
    public async Task Customer_cannot_call_admin_provider_mutations(string path)
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            $$"""
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('{{path}}', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({})
              });

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("updated", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("linked", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("blocked", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_read_admin_provider_location_status()
    {
        var response = await Page.APIRequest.GetAsync(
            $"/api/admin/providers/{FakeProvider}/location-status",
            new Microsoft.Playwright.APIRequestContextOptions
            {
                MaxRedirects = 0
            });

        Assert.Equal(403, response.Status);

        var body = await response.TextAsync();
        Assert.DoesNotContain("latitude", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("longitude", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("location_update_allowed", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
