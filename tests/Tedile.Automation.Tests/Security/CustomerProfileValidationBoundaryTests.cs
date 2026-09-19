using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class CustomerProfileValidationBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    [TestCase("12345")]
    [TestCase("test customer")]
    [TestCase("<script>alert(1)</script>")]
    public async Task Invalid_profile_name_is_rejected_without_changing_saved_name(string invalidName)
    {
        await Page.GotoAsync("/customer/dashboard");

        var beforeName = await ReadProfileNameAsync();

        var result = await Page.EvaluateAsync<JsonElement>(
            $$"""
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/customer/profile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({ name: {{JsonSerializer.Serialize(invalidName)}} })
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.Contains("valid name", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);

        var afterName = await ReadProfileNameAsync();
        Assert.Equal(beforeName, afterName);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Invalid_service_address_is_rejected_without_mutating_profile()
    {
        await Page.GotoAsync("/customer/dashboard");

        var beforeName = await ReadProfileNameAsync();

        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
              const response = await fetch('/customer/profile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                  'Accept': 'application/json',
                  'Content-Type': 'application/json',
                  'X-CSRFToken': csrf
                },
                body: JSON.stringify({
                  name: 'Google Play Review',
                  service_address: 'unknown'
                })
              });
              return { status: response.status, body: await response.text() };
            }
            """);

        Assert.Equal(400, result.GetProperty("status").GetInt32());

        var body = result.GetProperty("body").GetString() ?? string.Empty;
        Assert.Contains("valid service address", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);

        var afterName = await ReadProfileNameAsync();
        Assert.Equal(beforeName, afterName);
    }

    private async Task<string> ReadProfileNameAsync()
    {
        var result = await Page.EvaluateAsync<JsonElement>(
            """
            async () => {
              const response = await fetch('/customer/profile', {
                credentials: 'same-origin',
                cache: 'no-store',
                headers: { 'Accept': 'application/json' }
              });
              return await response.json();
            }
            """);

        return result.TryGetProperty("name", out var name)
            ? name.GetString() ?? string.Empty
            : string.Empty;
    }
}
