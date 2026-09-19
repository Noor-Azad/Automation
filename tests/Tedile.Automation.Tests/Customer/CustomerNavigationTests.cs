using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

[Category("WebKitSmoke")]
[Category("FirefoxSmoke")]
public sealed class CustomerNavigationTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_secondary_screen_back_buttons_are_deterministic()
    {
        await Page.GotoAsync("/customer/dashboard");
        var dashboard = new CustomerDashboardPage(Page);

        Logger.Step("Open Services from Home and return using the in-app Back button.");
        await dashboard.OpenServicesFromHomeAsync();
        await Expect(dashboard.ServicesScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
        await dashboard.BackFromServicesAsync();
        await Expect(dashboard.HomeScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));

        Logger.Step("Open Account > Personal Information and return using the in-app Back button.");
        await dashboard.OpenAccountAsync();
        await Expect(dashboard.AccountScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
        await dashboard.OpenPersonalInformationAsync();
        await Expect(dashboard.ProfileEditScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
        await dashboard.BackFromProfileEditAsync();
        await Expect(dashboard.AccountScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
    }
}
