using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class CsrfMutationBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Booking_create_without_csrf_is_rejected_before_validation_or_mutation()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const body = new URLSearchParams({
                provider_profile_code: 'AUTOMATION-NOT-A-REAL-PROVIDER',
                service_slug: 'automation-not-a-real-service',
                notes: 'csrf-boundary-check'
              });

              const response = await fetch('/customer/bookings', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/x-www-form-urlencoded'
                },
                body
              });

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("Booking created", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("provider", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("service", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Notification_clear_without_csrf_is_rejected_before_destructive_action()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/notifications/clear', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Accept': 'application/json' }
              });

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("cleared", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("deleted", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
