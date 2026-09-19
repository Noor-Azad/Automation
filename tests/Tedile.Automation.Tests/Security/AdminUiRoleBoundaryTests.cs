using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class AdminUiRoleBoundaryTests : AuthenticatedCustomerBaseTest
{
    [TestCase("/admin/providers/new")]
    [TestCase("/admin/providers/AUTOMATION-NOT-A-REAL-PROVIDER")]
    [TestCase("/admin/providers/2147483647/documents/service_agreement/download")]
    public async Task Customer_cannot_open_admin_provider_pages_or_documents(string path)
    {
        var response = await Page.APIRequest.GetAsync(
            path,
            new Microsoft.Playwright.APIRequestContextOptions
            {
                MaxRedirects = 0
            });

        Assert.Equal(403, response.Status);

        var body = await response.TextAsync();
        Assert.DoesNotContain("Provider management", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("document", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Customer_cannot_review_provider_documents()
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const body = new URLSearchParams({
                review_status: 'approved',
                review_notes: 'automation authorization boundary'
              });

              const response = await fetch(
                '/admin/providers/2147483647/documents/service_agreement/review',
                {
                  method: 'POST',
                  credentials: 'same-origin',
                  headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-CSRFToken': csrf
                  },
                  body
                }
              );

              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(403, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.DoesNotContain("approved", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("reviewed", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
    }
}
