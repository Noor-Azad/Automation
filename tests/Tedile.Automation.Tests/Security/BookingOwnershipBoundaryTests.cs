using System.Text.Json;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class BookingOwnershipBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Unknown_booking_tracking_returns_clean_not_found()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync(
            "/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/tracking");

        Assert.NotNull(response);
        Assert.Equal(404, response!.Status);

        var body = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(body);

        Assert.Equal("Booking not found", json.RootElement.GetProperty("error").GetString());
        Assert.DoesNotContain("latitude", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("longitude", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Unknown_booking_review_returns_clean_not_found_with_valid_csrf()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const body = new URLSearchParams({
                rating: '5',
                comment: 'automation-boundary-check'
              });
              const response = await fetch(
                '/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/review',
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

        Assert.Equal(404, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.Contains("Booking not found", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Booking_review_without_csrf_is_rejected_before_any_mutation()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const body = new URLSearchParams({
                rating: '5',
                comment: 'automation-csrf-check'
              });
              const response = await fetch(
                '/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/review',
                {
                  method: 'POST',
                  credentials: 'same-origin',
                  headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/x-www-form-urlencoded'
                  },
                  body
                }
              );
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("Review submitted", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
