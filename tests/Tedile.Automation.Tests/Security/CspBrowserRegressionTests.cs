using Microsoft.Playwright;
using Tedile.Automation.Core;
using Tedile.Automation.Pages;

namespace Tedile.Automation.Tests.Security;

[Category("WebKitSmoke")]
[Category("FirefoxSmoke")]
public sealed class CspBrowserRegressionTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_dashboard_has_no_CSP_script_execution_errors()
    {
        var violations = new List<string>();
        Page.Console += (_, message) =>
        {
            if (message.Type == "error" &&
                message.Text.Contains("Content Security Policy", StringComparison.OrdinalIgnoreCase))
            {
                violations.Add(message.Text);
            }
        };

        await Page.GotoAsync("/customer/dashboard");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        Assert.Empty(violations);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Review_OTP_page_has_no_CSP_script_execution_errors()
    {
        var settings = Fixture.Settings;

        // This class uses the authenticated customer storage state for the
        // dashboard CSP test. Clear that session before exercising the public
        // login/OTP flow, otherwise "/" redirects straight back to the
        // customer dashboard and the welcome-page controls never exist.
        await Context.ClearCookiesAsync();

        var violations = new List<string>();
        Page.Console += (_, message) =>
        {
            if (message.Type == "error" &&
                message.Text.Contains("Content Security Policy", StringComparison.OrdinalIgnoreCase))
            {
                violations.Add(message.Text);
            }
        };

        var welcome = new WelcomePage(Page);
        await welcome.OpenAsync();
        await welcome.RequestCustomerOtpAsync(settings.CustomerPhone!);
        await Page.WaitForURLAsync("**/otp");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        Assert.Empty(violations);
    }
}
