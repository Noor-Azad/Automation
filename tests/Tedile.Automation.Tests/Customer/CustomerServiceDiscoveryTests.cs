using Microsoft.Playwright;
using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerServiceDiscoveryTests : AuthenticatedCustomerBaseTest, IClassFixture<PlaywrightFixture>
{
    public CustomerServiceDiscoveryTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Authenticated_customer_can_load_and_filter_service_catalogue()
    {
        await Page.GotoAsync("/customer/dashboard");
        var dashboard = new CustomerDashboardPage(Page);

        await dashboard.OpenServicesFromHomeAsync();
        await Expect(dashboard.ServicesScreen).ToHaveClassAsync(
            new System.Text.RegularExpressions.Regex("active"));

        await dashboard.WaitForServiceCatalogueAsync();
        Assert.True(await dashboard.ServiceCards.CountAsync() > 0);

        var firstName = (await dashboard.ServiceCards.First.Locator(".service-name").InnerTextAsync()).Trim();
        Assert.False(string.IsNullOrWhiteSpace(firstName));

        var query = firstName.Length >= 3 ? firstName[..3] : firstName;
        await dashboard.FilterServicesAsync(query);

        await Expect(dashboard.ServiceCards.Filter(new() { HasText = firstName }).First).ToBeVisibleAsync();
    }

    [RequiresCustomerCredentialsFact]
    public async Task Selecting_a_service_calls_provider_search_without_error()
    {
        await Page.GotoAsync("/customer/dashboard");
        var dashboard = new CustomerDashboardPage(Page);

        await dashboard.OpenServicesFromHomeAsync();
        await dashboard.WaitForServiceCatalogueAsync();

        var responseTask = Page.WaitForResponseAsync(response =>
            response.Url.Contains("/api/search/providers?", StringComparison.Ordinal) &&
            response.Request.Method == "GET");

        await dashboard.SelectFirstServiceAsync();

        var response = await responseTask;
        Assert.Equal(200, response.Status);

        await Expect(dashboard.ServiceResultsScreen).ToHaveClassAsync(
            new System.Text.RegularExpressions.Regex("active"));
        await Expect(dashboard.ProviderResults.Locator(".error-state")).ToHaveCountAsync(0);
        await Expect(dashboard.ResultsSubtitle).ToBeVisibleAsync();
    }
}
