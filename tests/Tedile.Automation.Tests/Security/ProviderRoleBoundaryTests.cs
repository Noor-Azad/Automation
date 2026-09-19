using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class ProviderRoleBoundaryTests : AuthenticatedCustomerBaseTest
{
    [TestCase("/provider/profile/registered-location", "POST")]
    [TestCase("/provider/availability", "POST")]
    [TestCase("/provider/working-hours", "PUT")]
    [TestCase("/provider/bookings/AUTOMATION-NOT-A-REAL-BOOKING/quote", "POST")]
    public async Task Customer_cannot_call_provider_only_mutations(string path, string method)
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            $$"""
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('{{path}}', {
                method: '{{method}}',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({
                  availability: 'available',
                  latitude: 25.0,
                  longitude: 88.0,
                  amount: 100
                })
              });

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("updated", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("saved", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("quoted", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_read_provider_working_hours_owner_endpoint()
    {
        var response = await Page.APIRequest.GetAsync(
            "/provider/working-hours",
            new Microsoft.Playwright.APIRequestContextOptions
            {
                MaxRedirects = 0
            });

        Assert.Equal(403, response.Status);

        var body = await response.TextAsync();
        Assert.DoesNotContain("days", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("configured", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
