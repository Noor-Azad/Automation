using Microsoft.Playwright;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Security;

public sealed class SessionCookieSecurityTests : AuthenticatedCustomerBaseTest, IClassFixture<PlaywrightFixture>
{
    public SessionCookieSecurityTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Authenticated_session_cookie_has_secure_flags()
    {
        await Page.GotoAsync("/customer/dashboard");

        var cookies = await Context.CookiesAsync();
        var session = cookies.FirstOrDefault(cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));

        Assert.NotNull(session);
        Assert.True(session!.Secure);
        Assert.True(session.HttpOnly);
        Assert.Equal(SameSiteAttribute.Lax, session.SameSite);
    }

    [RequiresCustomerCredentialsFact]
    public async Task Session_cookie_is_scoped_to_tedile_host()
    {
        await Page.GotoAsync("/customer/dashboard");

        var host = new Uri(Fixture.Settings.BaseUrl).Host;
        var cookies = await Context.CookiesAsync();
        var session = cookies.First(cookie =>
            string.Equals(cookie.Name, "session", StringComparison.Ordinal));

        Assert.Contains(host, session.Domain, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("/", session.Path);
    }
}
