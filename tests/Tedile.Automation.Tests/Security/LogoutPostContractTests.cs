using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class LogoutPostContractTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    [NonParallelizable]
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

        // Submit logout from the page with the browser's fetch API and allow
        // the redirect to be followed. Flask clears/replaces the session cookie
        // on the redirect response, and the browser cookie jar must process that
        // Set-Cookie before we verify the session state.
        int logoutStatus;
        try
        {
            logoutStatus = await Page.EvaluateAsync<int>(
                """
                async () => {
                  const response = await fetch('/logout', {
                    method: 'POST',
                    credentials: 'same-origin'
                  });
                  return response.status;
                }
                """);
        }
        finally
        {
            CustomerAuthStateManager.InvalidateCachedState();
        }

        Assert.Equal(200, logoutStatus);

        var after = await Page.EvaluateAsync<System.Text.Json.JsonElement>(
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
