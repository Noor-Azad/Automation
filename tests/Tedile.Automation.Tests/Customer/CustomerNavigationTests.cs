using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerNavigationTests : BaseTest, IClassFixture<PlaywrightFixture>
{
    public CustomerNavigationTests(PlaywrightFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Customer_secondary_screen_back_buttons_are_deterministic()
    {
        var dashboard = await LoginAsync();

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

    private async Task<CustomerDashboardPage> LoginAsync()
    {
        var settings = Fixture.Settings;
        var welcome = new WelcomePage(Page);
        var otp = new OtpPage(Page);

        await welcome.OpenAsync();
        await welcome.RequestCustomerOtpAsync(settings.CustomerPhone!);
        await Page.WaitForURLAsync("**/otp");
        await otp.VerifyAsync(settings.CustomerOtp!);
        await Page.WaitForURLAsync("**/customer/dashboard");
        return new CustomerDashboardPage(Page);
    }
}
