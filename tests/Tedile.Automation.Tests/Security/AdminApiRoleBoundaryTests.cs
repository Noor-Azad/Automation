using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class AdminApiRoleBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_create_provider_through_admin_api()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/api/admin/providers', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({
                  name: 'Automation Should Not Create',
                  city: 'Kolkata'
                })
              });

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("created", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_update_provider_through_admin_api()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch(
                '/api/admin/providers/AUTOMATION-NOT-A-REAL-PROVIDER',
                {
                  method: 'PATCH',
                  credentials: 'same-origin',
                  headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'X-CSRFToken': csrf
                  },
                  body: JSON.stringify({ bio: 'Should not be applied' })
                }
              );

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("updated", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
