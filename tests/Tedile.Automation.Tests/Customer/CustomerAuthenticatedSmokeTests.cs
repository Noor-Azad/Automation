using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerAuthenticatedSmokeTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Authenticated_customer_can_open_primary_sections()
    {
        await Page.GotoAsync("/customer/dashboard");
        var dashboard = new CustomerDashboardPage(Page);

        await Expect(dashboard.HomeScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
        await Expect(dashboard.Greeting).ToBeVisibleAsync();

        await dashboard.OpenBookingsAsync();
        await Expect(dashboard.BookingsScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));

        await dashboard.OpenAccountAsync();
        await Expect(dashboard.AccountScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));

        await dashboard.OpenPersonalInformationAsync();
        await Expect(dashboard.ProfileEditScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
        await Expect(dashboard.ProfilePhone).ToBeVisibleAsync();
        Assert.NotNull(await dashboard.ProfilePhone.GetAttributeAsync("readonly"));
    }

    [Test, RequiresCustomerCredentials]
    public async Task Authenticated_customer_can_logout()
    {
        await Page.GotoAsync("/customer/dashboard");
        var dashboard = new CustomerDashboardPage(Page);

        await dashboard.OpenAccountAsync();
        await dashboard.LogoutAsync();

        await Expect(Page.Locator("body")).Not.ToHaveClassAsync(
            new System.Text.RegularExpressions.Regex("customer-mobile-app"));
        await Expect(Page.Locator("[data-auth-form='customer-login']")).ToBeVisibleAsync();
    }
}
