using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class ProviderOwnerBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_open_provider_profile_edit()
    {
        var response = await Page.APIRequest.GetAsync(
            "/provider/profile/edit",
            new Microsoft.Playwright.APIRequestContextOptions
            {
                MaxRedirects = 0
            });

        Assert.Equal(403, response.Status);

        var body = await response.TextAsync();
        Assert.DoesNotContain("Operational location", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Save", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_update_provider_booking_status()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const body = new URLSearchParams({ status: 'completed' });

              const response = await fetch(
                '/provider/bookings/AUTOMATION-NOT-A-REAL-BOOKING/status',
                {
                  method: 'POST',
                  credentials: 'same-origin',
                  headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-CSRFToken': csrf
                  },
                  body
                }
              );

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("completed", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("updated", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
