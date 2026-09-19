using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Security;

public sealed class HttpMethodBoundaryTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Logout_endpoint_rejects_get_requests()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync("/logout");
        Assert.NotNull(response);
        Assert.Equal(405, response!.Status);

        Assert.Contains("customer", await ReadSessionBodyAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Booking_create_endpoint_rejects_get_requests()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync("/customer/bookings");
        Assert.NotNull(response);
        Assert.Equal(405, response!.Status);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Booking_review_endpoint_rejects_get_requests()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync(
            "/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/review");

        Assert.NotNull(response);
        Assert.Equal(405, response!.Status);
    }

    [Test, RequiresCustomerCredentials]
    public async Task Booking_quote_endpoint_rejects_get_requests()
    {
        await Page.GotoAsync("/customer/dashboard");

        var response = await Page.GotoAsync(
            "/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/quote");

        Assert.NotNull(response);
        Assert.Equal(405, response!.Status);
    }

    private async Task<string> ReadSessionBodyAsync()
    {
        var response = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, response.Status);
        return await response.TextAsync();
    }
}
