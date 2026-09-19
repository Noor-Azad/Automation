using System.Text.Json;
using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerProviderProfileTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Authenticated_customer_can_open_real_provider_profile()
    {
        var providerCode = await FindBookableProviderCodeAsync();
        await Page.GotoAsync($"/providers/{Uri.EscapeDataString(providerCode)}");

        var profile = new ProviderProfilePage(Page);
        await profile.WaitForLoadedAsync();

        await Expect(profile.Heading).ToBeVisibleAsync();
        await Expect(profile.Location).ToBeVisibleAsync();
        Assert.True(await profile.Services.CountAsync() > 0);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Booking_sheet_exposes_verified_readonly_phone_without_submitting_booking()
    {
        var providerCode = await FindBookableProviderCodeAsync();
        await Page.GotoAsync($"/providers/{Uri.EscapeDataString(providerCode)}");

        var profile = new ProviderProfilePage(Page);
        await profile.WaitForLoadedAsync();
        await profile.OpenBookingPanelAsync();

        await Expect(profile.BookingForm).ToBeVisibleAsync();
        Assert.NotNull(await profile.PhoneField.GetAttributeAsync("readonly"));
        await Expect(profile.ScheduledAt).ToBeVisibleAsync();
        await Expect(profile.SubmitButton).ToBeVisibleAsync();

        // Deliberately do not submit against production.
        Assert.True(await profile.ServiceSelect.CountAsync() == 0 || await profile.ServiceSelect.IsVisibleAsync());
    }

    private async Task<string> FindBookableProviderCodeAsync()
    {
        await Page.GotoAsync("/api/search/providers?limit=50&offset=0&sort=rating-high");
        var body = await Page.Locator("body").InnerTextAsync();

        using var json = JsonDocument.Parse(body);
        var providers = json.RootElement.GetProperty("data").GetProperty("providers");

        foreach (var provider in providers.EnumerateArray())
        {
            var availability = provider.TryGetProperty("availability", out var availabilityElement)
                ? availabilityElement.GetString()
                : null;
            if (string.Equals(availability, "offline", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (provider.TryGetProperty("id", out var idElement))
            {
                var id = idElement.ValueKind == JsonValueKind.String
                    ? idElement.GetString()
                    : idElement.ToString();

                if (!string.IsNullOrWhiteSpace(id))
                {
                    return id;
                }
            }
        }

        throw new AssertionException(
            "No non-offline provider is currently available in the production provider catalogue.");
    }
}
