using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerAuthenticationNegativeTests : BaseTest, IClassFixture<PlaywrightFixture>
{
    public CustomerAuthenticationNegativeTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Review_customer_invalid_otp_is_rejected()
    {
        var settings = Fixture.Settings;
        var welcome = new WelcomePage(Page);
        var otp = new OtpPage(Page);

        await welcome.OpenAsync();
        await welcome.RequestCustomerOtpAsync(settings.CustomerPhone!);
        await Page.WaitForURLAsync("**/otp");
        await Expect(otp.Heading).ToHaveTextAsync("Enter OTP");

        var invalidOtp = BuildDifferentOtp(settings.CustomerOtp!);
        await otp.VerifyAsync(invalidOtp);

        await Expect(otp.Error).ToHaveTextAsync("Invalid verification code.");
        Assert.Contains("/otp", Page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Change_phone_discards_otp_challenge_and_returns_to_welcome()
    {
        var settings = Fixture.Settings;
        var welcome = new WelcomePage(Page);
        var otp = new OtpPage(Page);

        await welcome.OpenAsync();
        await welcome.RequestCustomerOtpAsync(settings.CustomerPhone!);
        await Page.WaitForURLAsync("**/otp");

        await otp.ChangePhoneButton.ClickAsync();

        await Page.WaitForURLAsync("**/");
        await Expect(welcome.CustomerPhone).ToBeVisibleAsync();
        await Expect(welcome.CustomerPhone).ToHaveValueAsync(string.Empty);
    }

    private static string BuildDifferentOtp(string configuredOtp)
    {
        var otp = configuredOtp.Trim();
        if (otp.Length != 6 || !otp.All(char.IsDigit))
        {
            throw new Xunit.Sdk.XunitException(
                "TEDILE_E2E_CUSTOMER_OTP must be a 6-digit value for the review-account authentication tests.");
        }

        var chars = otp.ToCharArray();
        chars[^1] = chars[^1] == '9' ? '0' : (char)(chars[^1] + 1);
        return new string(chars);
    }
}
