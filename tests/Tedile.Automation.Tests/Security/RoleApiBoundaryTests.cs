using Microsoft.Playwright;
using System.Text.Json;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class RoleApiBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_update_provider_operational_location()
    {
        await Page.GotoAsync(
            "/customer/dashboard",
            new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/api/providers/me/location', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({
                  latitude: 25.0100,
                  longitude: 88.1400,
                  accuracy_meters: 10
                })
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("updated", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_access_provider_owner_dashboard_data()
    {
        await Page.GotoAsync(
            "/customer/dashboard",
            new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        var response = await Page.GotoAsync("/provider/dashboard");
        Assert.NotNull(response);
        Assert.Equal(403, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.DoesNotContain("Provider management", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Operational location", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_contact_lookup_for_unknown_provider_returns_clean_not_found()
    {
        await Page.GotoAsync(
            "/customer/dashboard",
            new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        var response = await Page.GotoAsync("/customer/providers/AUTOMATION-NOT-A-REAL-PROVIDER/contact");
        Assert.NotNull(response);
        Assert.Equal(404, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);

        Assert.Equal("Provider not found", json.RootElement.GetProperty("error").GetString());
        Assert.DoesNotContain("phone", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("whatsapp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
