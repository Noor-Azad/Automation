using System.Text.Json;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Security;

public sealed class SensitiveResponseCacheTests : AuthenticatedCustomerBaseTest, IClassFixture<PlaywrightFixture>
{
    public SensitiveResponseCacheTests(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [RequiresCustomerCredentialsFact]
    [Theory]
    [InlineData("/customer/dashboard")]
    [InlineData("/api/session")]
    [InlineData("/customer/profile")]
    public async Task Authenticated_sensitive_get_responses_are_not_cacheable(string path)
    {
        await Page.GotoAsync("/customer/dashboard");

        var result = await Page.EvaluateAsync<JsonElement>(
            $$"""
            async () => {
              const response = await fetch('{{path}}', {
                credentials: 'same-origin',
                cache: 'no-store',
                headers: { 'Accept': 'application/json, text/html' }
              });
              return {
                status: response.status,
                cacheControl: response.headers.get('cache-control') || ''
              };
            }
            """);

        Assert.Equal(200, result.GetProperty("status").GetInt32());
        Assert.Contains(
            "no-store",
            result.GetProperty("cacheControl").GetString() ?? string.Empty,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Public_root_is_not_cacheable_because_response_depends_on_session_state()
    {
        await Context.ClearCookiesAsync();

        var response = await Page.GotoAsync("/");
        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        Assert.True(response.Headers.TryGetValue("cache-control", out var cacheControl));
        Assert.Contains("no-store", cacheControl, StringComparison.OrdinalIgnoreCase);
    }
}
