using System.Text.Json;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class NotificationSecurityBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Authenticated_notifications_index_returns_scoped_contract()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync("/notifications");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;

        Assert.True(root.TryGetProperty("data", out var data));
        Assert.Equal(JsonValueKind.Array, data.ValueKind);

        Assert.True(root.TryGetProperty("unread", out var unread));
        Assert.True(unread.GetInt32() >= 0);

        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("otp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password_hash", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Unknown_notification_cannot_be_marked_read_even_with_valid_csrf()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/notifications/2147483647/read', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'X-CSRFToken': csrf
                }
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(404, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.Contains("Notification not found", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Notification_mark_read_without_csrf_is_rejected_before_lookup()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/notifications/2147483647/read', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Accept': 'application/json' }
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("Notification not found", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    [TestCase("/notifications/2147483647/read")]
    [TestCase("/notifications/clear")]
    public async Task Notification_mutation_endpoints_reject_get(string path)
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync(path);
        Assert.NotNull(response);
        Assert.Equal(405, response!.Status);
    }
}
