using Tedile.Automation.Api;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests.Api;

public sealed class HealthApiTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private readonly ITestOutputHelper _output;

    public HealthApiTests(PlaywrightFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task Health_endpoint_reports_tedile_as_healthy()
    {
        await _fixture.InitializeAsync();
        await using var api = await TedileApiClient.CreateAsync(_fixture.Playwright, _fixture.Settings.BaseUrl);
        var result = await api.GetHealthAsync();

        _output.WriteLine($"status={result.Status} app={result.Body.App} commit={result.Body.Commit ?? "<null>"}");
        Assert.Equal(200, result.Status);
        Assert.Equal("healthy", result.Body.Status);
        Assert.Equal("Tedile", result.Body.App);
    }
}
