using Microsoft.Playwright;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Security;

public sealed class PublicSecurityTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private readonly ITestOutputHelper _output;

    public PublicSecurityTests(PlaywrightFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task Public_html_has_expected_security_headers()
    {
        await _fixture.InitializeAsync();
        await using var request = await _fixture.Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = _fixture.Settings.BaseUrl
        });

        var response = await request.GetAsync("/");
        Assert.Equal(200, response.Status);

        var headers = response.Headers;
        Assert.Equal("nosniff", headers["x-content-type-options"]);
        Assert.Equal("DENY", headers["x-frame-options"]);
        Assert.Equal("strict-origin-when-cross-origin", headers["referrer-policy"]);
        Assert.Contains("frame-ancestors 'none'", headers["content-security-policy"]);
        Assert.Contains("object-src 'none'", headers["content-security-policy"]);
        Assert.DoesNotContain("'unsafe-eval'", headers["content-security-policy"]);

        _output.WriteLine(headers["content-security-policy"]);
    }

    [Theory]
    [InlineData("/customer/dashboard")]
    [InlineData("/provider/dashboard")]
    [InlineData("/admin/dashboard")]
    public async Task Anonymous_user_cannot_open_protected_dashboards(string path)
    {
        await _fixture.InitializeAsync();
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync(path);

        Assert.DoesNotContain(path, new Uri(page.Url).AbsolutePath, StringComparison.OrdinalIgnoreCase);
        Assert.True(new Uri(page.Url).AbsolutePath is "/" or "/login",
            $"Expected protected route to redirect to login/welcome, but landed on {page.Url}");
    }

    [Theory]
    [InlineData("0 OR 1=1", "77.1025")]
    [InlineData("25.0", "77.0' OR '1'='1")]
    [InlineData("NaN", "77.1025")]
    [InlineData("91", "77.1025")]
    public async Task Service_search_rejects_malformed_or_injection_like_coordinates(string latitude, string longitude)
    {
        await _fixture.InitializeAsync();
        await using var request = await _fixture.Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = _fixture.Settings.BaseUrl,
            ExtraHTTPHeaders = new Dictionary<string, string> { ["Accept"] = "application/json" }
        });

        var url = $"/api/services?latitude={Uri.EscapeDataString(latitude)}&longitude={Uri.EscapeDataString(longitude)}";
        var response = await request.GetAsync(url);
        var body = await response.TextAsync();

        Assert.Equal(400, response.Status);
        Assert.DoesNotContain("Traceback", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sqlalchemy", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("psycopg", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Session_api_does_not_expose_authenticated_data_to_anonymous_user()
    {
        await _fixture.InitializeAsync();
        await using var request = await _fixture.Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = _fixture.Settings.BaseUrl,
            ExtraHTTPHeaders = new Dictionary<string, string> { ["Accept"] = "application/json" }
        });

        var response = await request.GetAsync("/api/session");
        Assert.Equal(401, response.Status);

        var body = await response.TextAsync();
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", body, StringComparison.OrdinalIgnoreCase);
    }
}
