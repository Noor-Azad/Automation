using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerAuthenticationTests : BaseTest, IClassFixture<PlaywrightFixture>
{
    public CustomerAuthenticationTests(PlaywrightFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Review_customer_can_complete_otp_login_and_reach_dashboard()
    {
        var settings = Fixture.Settings;
        var welcome = new WelcomePage(Page);
        var otp = new OtpPage(Page);
        var dashboard = new CustomerDashboardPage(Page);

        await welcome.OpenAsync();
        await welcome.RequestCustomerOtpAsync(settings.CustomerPhone!);
        await Page.WaitForURLAsync("**/otp");
        await Expect(otp.Heading).ToHaveTextAsync("Enter OTP");

        await otp.VerifyAsync(settings.CustomerOtp!);
        await Page.WaitForURLAsync("**/customer/dashboard");

        await Expect(dashboard.HomeScreen).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
        await Expect(dashboard.Greeting).ToBeVisibleAsync();
    }
}
