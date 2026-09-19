using System.Text.Json;
using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Security;

public sealed class LogoutSessionInvalidationTests : AuthenticatedCustomerBaseTest, IClassFixture<PlaywrightFixture>
{
    public LogoutSessionInvalidationTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Logout_invalidates_authenticated_session_api_access()
    {
        await Page.GotoAsync("/customer/dashboard");

        var before = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/api/session', {
                credentials: 'same-origin',
                headers: { 'Accept': 'application/json' }
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(200, before.GetProperty("status").GetInt32());
        Assert.Contains(
            "\"authenticated\":true",
            (before.GetProperty("body").GetString() ?? string.Empty).Replace(" ", string.Empty),
            StringComparison.OrdinalIgnoreCase);

        var dashboard = new CustomerDashboardPage(Page);
        await dashboard.OpenAccountAsync();
        await dashboard.LogoutAsync();

        var after = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/api/session', {
                credentials: 'same-origin',
                headers: { 'Accept': 'application/json' }
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(401, after.GetProperty("status").GetInt32());

        var body = after.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("otp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("phone", body, StringComparison.OrdinalIgnoreCase);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Logout_replaces_authenticated_session_cookie_with_anonymous_session()
    {
        await Page.GotoAsync("/customer/dashboard");

        var before = await Context.CookiesAsync();
        var authenticatedSession = before.FirstOrDefault(cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));

        Assert.NotNull(authenticatedSession);
        var authenticatedValue = authenticatedSession!.Value;

        var dashboard = new CustomerDashboardPage(Page);
        await dashboard.OpenAccountAsync();
        await dashboard.LogoutAsync();

        var after = await Context.CookiesAsync();
        var anonymousSession = after.FirstOrDefault(cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));

        // The login page renders a CSRF token, so Flask may immediately create
        // a fresh anonymous session cookie after logout. The security property
        // is that the authenticated cookie value is no longer reusable.
        if (anonymousSession is not null)
        {
            Assert.NotEqual(authenticatedValue, anonymousSession.Value);
        }

        var sessionResponse = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(401, sessionResponse.Status);
    }
}
