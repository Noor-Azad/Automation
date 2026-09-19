using System.Text.Json;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class AuthenticatedCustomerSecurityTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_profile_get_does_not_expose_private_identity_fields()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/customer/profile', {
                credentials: 'same-origin',
                cache: 'no-store'
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(200, result.GetProperty("status").GetInt32());
        var body = result.GetProperty("body").GetString() ?? string.Empty;
        var lower = body.ToLowerInvariant();

        Assert.Contains("\"name\"", lower);
        Assert.DoesNotContain("phone", lower);
        Assert.DoesNotContain("email", lower);
        Assert.DoesNotContain("password", lower);
        Assert.DoesNotContain("otp", lower);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Profile_update_without_csrf_is_rejected_and_does_not_change_name()
    {
        await Page.GotoAsync("/customer/dashboard");

        var before = await ReadProfileNameAsync();

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/customer/profile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: 'Should Not Be Applied' })
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());

        var after = await ReadProfileNameAsync();
        Assert.Equal(before, after);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_change_phone_through_profile_endpoint()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/customer/profile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({ phone: '+919876543210' })
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());
        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.Contains("Phone number cannot be changed here.", body, StringComparison.Ordinal);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Provider_directions_are_denied_outside_active_booking_journey()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/customer/providers/NOT-A-REAL-PROVIDER/directions', {
                credentials: 'same-origin',
                headers: { 'Accept': 'application/json' }
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());
        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.Contains("active booking journey", body, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<string> ReadProfileNameAsync()
    {
        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/customer/profile', {
                credentials: 'same-origin',
                cache: 'no-store'
              });
              return await response.json();
            }
            """);

        return result.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty;
    }
}
