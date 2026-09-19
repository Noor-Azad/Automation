using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class LogoutPostContractTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Logout_post_without_csrf_clears_only_the_current_session()
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

        var logout = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/logout', {
                method: 'POST',
                credentials: 'same-origin',
                redirect: 'manual'
              });
              return {
                status: response.status,
                location: response.headers.get('location') || ''
              };
            }
            """);

        var status = logout.GetProperty("status").GetInt32();
        Assert.True(status is 0 or 302 or 303);

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
        Assert.DoesNotContain("phone", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("otp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Logout_get_remains_disallowed_and_does_not_end_session()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync("/logout");
        Assert.NotNull(response);
        Assert.Equal(405, response!.Status);

        var session = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, session.Status);
    }
}
