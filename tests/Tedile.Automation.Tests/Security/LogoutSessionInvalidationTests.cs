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
    public async Task Logout_removes_active_session_cookie_from_browser_context()
    {
        await Page.GotoAsync("/customer/dashboard");

        var before = await Context.CookiesAsync();
        Assert.Contains(before, cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));

        var dashboard = new CustomerDashboardPage(Page);
        await dashboard.OpenAccountAsync();
        await dashboard.LogoutAsync();

        var after = await Context.CookiesAsync();
        Assert.DoesNotContain(after, cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));
    }
}
