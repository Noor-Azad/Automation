using System.Text.Json;
using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerBookingValidationTests : AuthenticatedCustomerBaseTest, IClassFixture<PlaywrightFixture>
{
    public CustomerBookingValidationTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Invalid_customer_name_is_blocked_before_booking_request_is_sent()
    {
        var profile = await OpenBookableProviderAsync();
        await profile.OpenBookingPanelAsync();

        var bookingPosts = 0;
        Page.Request += (_, request) =>
        {
            if (request.Method == "POST" &&
                new Uri(request.Url).AbsolutePath.Equals("/customer/bookings", StringComparison.Ordinal))
            {
                Interlocked.Increment(ref bookingPosts);
            }
        };

        await profile.CustomerName.FillAsync("12345");
        await profile.SubmitButton.ClickAsync();

        await Expect(profile.NameError).ToHaveTextAsync("Please enter a valid name using letters, spaces, and dots only.");
        Assert.Equal(0, bookingPosts);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Invalid_service_address_is_blocked_before_booking_request_is_sent()
    {
        var profile = await OpenBookableProviderAsync();
        await profile.OpenBookingPanelAsync();

        var bookingPosts = 0;
        Page.Request += (_, request) =>
        {
            if (request.Method == "POST" &&
                new Uri(request.Url).AbsolutePath.Equals("/customer/bookings", StringComparison.Ordinal))
            {
                Interlocked.Increment(ref bookingPosts);
            }
        };

        await profile.CustomerName.FillAsync("Automation Customer");
        await profile.AddressLine1.FillAsync("test");
        await profile.AddressLocality.FillAsync("Malda");
        await profile.AddressPincode.FillAsync("732101");
        await profile.SubmitButton.ClickAsync();

        await Expect(profile.AddressError).ToHaveTextAsync("Please enter a proper service address.");
        Assert.Equal(0, bookingPosts);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Invalid_pincode_is_blocked_before_booking_request_is_sent()
    {
        var profile = await OpenBookableProviderAsync();
        await profile.OpenBookingPanelAsync();

        var bookingPosts = 0;
        Page.Request += (_, request) =>
        {
            if (request.Method == "POST" &&
                new Uri(request.Url).AbsolutePath.Equals("/customer/bookings", StringComparison.Ordinal))
            {
                Interlocked.Increment(ref bookingPosts);
            }
        };

        await profile.CustomerName.FillAsync("Automation Customer");
        await profile.AddressLine1.FillAsync("Station Road");
        await profile.AddressLocality.FillAsync("Malda");
        await profile.AddressPincode.FillAsync("1234");
        await profile.SubmitButton.ClickAsync();

        await Expect(profile.AddressError).ToHaveTextAsync("Enter a valid 6-digit PIN code.");
        Assert.Equal(0, bookingPosts);
    }

    private async Task<ProviderProfilePage> OpenBookableProviderAsync()
    {
        await Page.GotoAsync("/api/search/providers?limit=50&offset=0&sort=rating-high");
        var body = await Page.Locator("body").InnerTextAsync();

        using var json = JsonDocument.Parse(body);
        var providers = json.RootElement.GetProperty("data").GetProperty("providers");

        string? providerCode = null;
        foreach (var provider in providers.EnumerateArray())
        {
            var availability = provider.TryGetProperty("availability", out var availabilityElement)
                ? availabilityElement.GetString()
                : null;

            if (string.Equals(availability, "offline", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!provider.TryGetProperty("id", out var idElement))
            {
                continue;
            }

            providerCode = idElement.ValueKind == JsonValueKind.String
                ? idElement.GetString()
                : idElement.ToString();

            if (!string.IsNullOrWhiteSpace(providerCode))
            {
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(providerCode))
        {
            throw new Xunit.Sdk.XunitException(
                "No non-offline provider is currently available in the production provider catalogue.");
        }

        await Page.GotoAsync($"/providers/{Uri.EscapeDataString(providerCode)}");
        var profile = new ProviderProfilePage(Page);
        await profile.WaitForLoadedAsync();
        return profile;
    }
}
