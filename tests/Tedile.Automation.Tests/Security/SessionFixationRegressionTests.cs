using Tedile.Automation.Pages;

namespace Tedile.Automation.Tests.Security;

public sealed class SessionFixationRegressionTests : BaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Customer_login_rotates_anonymous_session_cookie()
    {
        await Context.ClearCookiesAsync();

        var welcome = new WelcomePage(Page);
        await welcome.OpenAsync();

        var anonymousCookies = await Context.CookiesAsync();
        var anonymousSession = anonymousCookies.FirstOrDefault(cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));

        Assert.NotNull(anonymousSession);
        var anonymousValue = anonymousSession!.Value;

        await welcome.RequestCustomerOtpAsync(Fixture.Settings.CustomerPhone!);
        await Page.WaitForURLAsync("**/otp");

        var otp = new OtpPage(Page);
        await otp.VerifyAsync(Fixture.Settings.CustomerOtp!);
        await Page.WaitForURLAsync("**/customer/dashboard");

        var authenticatedCookies = await Context.CookiesAsync();
        var authenticatedSession = authenticatedCookies.FirstOrDefault(cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));

        Assert.NotNull(authenticatedSession);
        Assert.NotEqual(anonymousValue, authenticatedSession!.Value);

        var response = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, response.Status);

        var body = await response.TextAsync();
        Assert.Contains("\"authenticated\":true", body.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
    }
}
