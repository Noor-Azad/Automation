using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class ProfileMassAssignmentBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Profile_update_cannot_mass_assign_privileged_identity_fields()
    {
        await Page.GotoAsync("/customer/dashboard");

        var before = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, before.Status);
        var beforeBody = await before.TextAsync();

        using var beforeJson = JsonDocument.Parse(beforeBody);
        var beforeUser = beforeJson.RootElement.GetProperty("user");
        var beforeRole = beforeUser.GetProperty("role").GetString() ?? string.Empty;
        var beforeRoles = beforeUser.GetProperty("roles")
            .EnumerateArray()
            .Select(x => x.GetString() ?? string.Empty)
            .ToArray();

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/customer/profile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({
                  role: 'admin',
                  roles: ['admin'],
                  is_admin: true,
                  is_test_account: true,
                  is_google_play_review_account: true,
                  onboarding_completed: true
                })
              });

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(200, result.GetProperty("status").GetInt32());

        var after = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, after.Status);
        var afterBody = await after.TextAsync();

        using var afterJson = JsonDocument.Parse(afterBody);
        var afterUser = afterJson.RootElement.GetProperty("user");

        Assert.Equal(beforeRole, afterUser.GetProperty("role").GetString() ?? string.Empty);

        var afterRoles = afterUser.GetProperty("roles")
            .EnumerateArray()
            .Select(x => x.GetString() ?? string.Empty)
            .ToArray();

        Assert.Equal(beforeRoles.Length, afterRoles.Length);
        foreach (var role in beforeRoles)
        {
            Assert.True(afterRoles.Contains(role, StringComparer.Ordinal));
        }

        Assert.DoesNotContain("\"role\":\"admin\"", afterBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("is_admin", afterBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("is_test_account", afterBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("is_google_play_review_account", afterBody, StringComparison.OrdinalIgnoreCase);
    }
}
