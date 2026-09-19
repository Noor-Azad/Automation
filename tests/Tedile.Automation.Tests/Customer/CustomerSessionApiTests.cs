using System.Text.Json;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerSessionApiTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Authenticated_session_api_reports_customer_identity_without_private_fields()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync("/api/session");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        var text = await Page.Locator("body").InnerTextAsync();
        using var json = JsonDocument.Parse(text);

        Assert.True(json.RootElement.GetProperty("authenticated").GetBoolean());

        var user = json.RootElement.GetProperty("user");
        Assert.Equal("customer", user.GetProperty("role").GetString());

        var body = text.ToLowerInvariant();
        Assert.DoesNotContain("password", body);
        Assert.DoesNotContain("phone", body);
        Assert.DoesNotContain("email", body);
        Assert.DoesNotContain("otp", body);
    }
}
