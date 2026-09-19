namespace Tedile.Automation.Tests.Security;

public sealed class AuthenticatedSecurityHeaderTests : AuthenticatedCustomerBaseTest
{
    [TestCase("/customer/dashboard")]
    [TestCase("/api/session")]
    [TestCase("/customer/profile")]
    [TestCase("/notifications")]
    public async Task Authenticated_sensitive_responses_keep_required_security_headers(string path)
    {
        var response = await Page.APIRequest.GetAsync(path);

        Assert.True(response.Status is 200 or 404);

        var headers = response.Headers;

        Assert.True(headers.TryGetValue("x-content-type-options", out var contentTypeOptions));
        Assert.Equal("nosniff", contentTypeOptions);

        Assert.True(headers.TryGetValue("x-frame-options", out var frameOptions));
        Assert.Equal("DENY", frameOptions);

        Assert.True(headers.TryGetValue("referrer-policy", out var referrerPolicy));
        Assert.Equal("strict-origin-when-cross-origin", referrerPolicy);

        Assert.True(headers.TryGetValue("permissions-policy", out var permissionsPolicy));
        Assert.Contains("camera=()", permissionsPolicy, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("microphone=()", permissionsPolicy, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("geolocation=(self)", permissionsPolicy, StringComparison.OrdinalIgnoreCase);

        Assert.True(headers.TryGetValue("content-security-policy", out var csp));
        Assert.Contains("object-src 'none'", csp, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("frame-ancestors 'none'", csp, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("base-uri 'self'", csp, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'unsafe-eval'", csp, StringComparison.OrdinalIgnoreCase);

        Assert.True(headers.TryGetValue("cache-control", out var cacheControl));
        Assert.Contains("no-store", cacheControl, StringComparison.OrdinalIgnoreCase);
    }
}
