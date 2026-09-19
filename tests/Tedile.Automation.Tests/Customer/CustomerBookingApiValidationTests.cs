using System.Text.Json;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerBookingApiValidationTests : AuthenticatedCustomerBaseTest, IClassFixture<PlaywrightFixture>
{
    public CustomerBookingApiValidationTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Booking_api_rejects_invalid_customer_name()
    {
        var result = await PostBookingAsync(new Dictionary<string, string>
        {
            ["customer_display_name"] = "12345",
            ["address_line_1"] = "Station Road",
            ["address_locality"] = "Malda",
            ["address_pincode"] = "732101",
            ["provider_profile_code"] = "NOT-A-REAL-PROVIDER",
            ["service_slug"] = "electrician"
        });

        Assert.Equal(400, result.Status);
        Assert.Contains("valid name", result.Body, StringComparison.OrdinalIgnoreCase);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Booking_api_rejects_invalid_structured_address()
    {
        var result = await PostBookingAsync(new Dictionary<string, string>
        {
            ["customer_display_name"] = "Automation Customer",
            ["address_line_1"] = "test",
            ["address_locality"] = "Malda",
            ["address_pincode"] = "732101",
            ["provider_profile_code"] = "NOT-A-REAL-PROVIDER",
            ["service_slug"] = "electrician"
        });

        Assert.Equal(400, result.Status);
        Assert.Contains("service address", result.Body, StringComparison.OrdinalIgnoreCase);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Booking_api_rejects_incomplete_coordinate_pair()
    {
        var result = await PostBookingAsync(new Dictionary<string, string>
        {
            ["customer_display_name"] = "Automation Customer",
            ["address_line_1"] = "Station Road",
            ["address_locality"] = "Malda",
            ["address_pincode"] = "732101",
            ["provider_profile_code"] = "NOT-A-REAL-PROVIDER",
            ["service_slug"] = "electrician",
            ["customer_latitude"] = "25.0"
        });

        Assert.Equal(400, result.Status);
        Assert.Contains("provided together", result.Body, StringComparison.OrdinalIgnoreCase);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Booking_api_rejects_non_finite_coordinates()
    {
        var result = await PostBookingAsync(new Dictionary<string, string>
        {
            ["customer_display_name"] = "Automation Customer",
            ["address_line_1"] = "Station Road",
            ["address_locality"] = "Malda",
            ["address_pincode"] = "732101",
            ["provider_profile_code"] = "NOT-A-REAL-PROVIDER",
            ["service_slug"] = "electrician",
            ["customer_latitude"] = "NaN",
            ["customer_longitude"] = "88.0"
        });

        Assert.Equal(400, result.Status);
        Assert.Contains("Invalid customer location coordinates", result.Body, StringComparison.OrdinalIgnoreCase);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Booking_api_rejects_oversized_notes_before_provider_lookup()
    {
        var result = await PostBookingAsync(new Dictionary<string, string>
        {
            ["customer_display_name"] = "Automation Customer",
            ["address_line_1"] = "Station Road",
            ["address_locality"] = "Malda",
            ["address_pincode"] = "732101",
            ["provider_profile_code"] = "NOT-A-REAL-PROVIDER",
            ["service_slug"] = "electrician",
            ["customer_latitude"] = "25.0",
            ["customer_longitude"] = "88.0",
            ["notes"] = new string('x', 5001)
        });

        Assert.Equal(400, result.Status);
        Assert.Contains("Notes are too long", result.Body, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<(int Status, string Body)> PostBookingAsync(Dictionary<string, string> values)
    {
        await Page.GotoAsync("/customer/dashboard");

        var payload = JsonSerializer.Serialize(values);
        var script = $$"""
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const values = {{payload}};
              const body = new URLSearchParams(values);
              const response = await fetch('/customer/bookings', {
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
            """;

        var result = await Page.EvaluateAsync<JsonElement>(script);
        return (
            result.GetProperty("status").GetInt32(),
            result.GetProperty("body").GetString() ?? string.Empty
        );
    }
}
