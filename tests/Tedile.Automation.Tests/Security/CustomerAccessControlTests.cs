using System.Text.Json;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class CustomerAccessControlTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_open_provider_dashboard()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync("/provider/dashboard");
        Assert.NotNull(response);
        Assert.Equal(403, response!.Status);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_open_admin_dashboard()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync("/admin/dashboard");
        Assert.NotNull(response);
        Assert.Equal(403, response!.Status);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Nonexistent_booking_cancel_is_hidden_as_not_found()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/cancel', {
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
        Assert.Contains("Booking not found", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Nonexistent_booking_quote_is_hidden_as_not_found()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const body = new URLSearchParams({
                action: 'accept',
                expected_amount: '100.00'
              });
              const response = await fetch('/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/quote', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/x-www-form-urlencoded',
                  'X-CSRFToken': csrf
                },
                body
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(404, result.GetProperty("status").GetInt32());
        var responseBody = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.Contains("Booking not found", responseBody, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Authenticated_customer_dashboard_is_not_cacheable()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/customer/dashboard', {
                credentials: 'same-origin',
                cache: 'no-store'
              });
              return {
                status: response.status,
                cacheControl: response.headers.get('cache-control') || ''
              };
            }
            """);

        Assert.Equal(200, result.GetProperty("status").GetInt32());
        Assert.Contains(
            "no-store",
            result.GetProperty("cacheControl").GetString() ?? string.Empty,
            StringComparison.OrdinalIgnoreCase);
    }
}
