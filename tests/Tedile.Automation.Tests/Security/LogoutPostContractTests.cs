namespace Tedile.Automation.Tests.Security;

public sealed class LogoutPostContractTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Logout_post_without_csrf_clears_only_the_current_session()
    {
        await Page.GotoAsync("/customer/dashboard");

        var before = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, before.Status);

        var beforeBody = await before.TextAsync();
        Assert.Contains(
            "\"authenticated\":true",
            beforeBody.Replace(" ", string.Empty),
            StringComparison.OrdinalIgnoreCase);

        // BrowserContext.APIRequest shares the browser context cookie jar.
        // POST directly so we can verify the logout contract without depending
        // on navigation timing or redirect handling in a browser engine.
        var logout = await Page.APIRequest.PostAsync(
            "/logout",
            new Microsoft.Playwright.APIRequestContextOptions
            {
                MaxRedirects = 0
            });

        Assert.Equal(302, logout.Status);

        var after = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(401, after.Status);

        var body = await after.TextAsync();
        Assert.DoesNotContain("phone", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("otp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Logout_get_remains_disallowed_and_does_not_end_session()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.APIRequest.GetAsync(
            "/logout",
            new Microsoft.Playwright.APIRequestContextOptions
            {
                MaxRedirects = 0
            });

        Assert.Equal(405, response.Status);

        var session = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, session.Status);
    }
}
