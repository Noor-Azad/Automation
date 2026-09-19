using Microsoft.Playwright;
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

        // Build a real browser form without a CSRF field, then let
        // Playwright click its submit button while waiting for navigation.
        // This avoids the execution-context race caused by calling form.submit()
        // from inside EvaluateAsync while the page is navigating away.
        await Page.EvaluateAsync(
            """
            () => {
              const form = document.createElement('form');
              form.id = 'automation-logout-form';
              form.method = 'POST';
              form.action = '/logout';

              const submit = document.createElement('button');
              submit.type = 'submit';
              submit.textContent = 'Automation logout';
              form.appendChild(submit);

              document.body.appendChild(form);
            }
            """);

        await Page.RunAndWaitForNavigationAsync(
            async () => await Page.Locator("#automation-logout-form button[type='submit']").ClickAsync(),
            new PageRunAndWaitForNavigationOptions
            {
                UrlString = "**/",
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

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
