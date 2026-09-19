using System.Text.Json;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Customer;

public sealed class CustomerSessionApiTests : AuthenticatedCustomerBaseTest, IClassFixture<PlaywrightFixture>
{
    public CustomerSessionApiTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    public async Task Authenticated_session_api_reports_customer_identity_without_private_fields()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, response.Status);

        var text = await response.TextAsync();
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
