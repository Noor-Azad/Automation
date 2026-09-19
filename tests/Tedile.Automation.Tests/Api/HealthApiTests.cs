using Tedile.Automation.Api;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests.Api;

public sealed class HealthApiTests : BaseTest
{

    [Test]
    public async Task Health_endpoint_reports_tedile_as_healthy()
    {
        await Fixture.InitializeAsync();
        await using var api = await TedileApiClient.CreateAsync(Fixture.Playwright, Fixture.Settings.BaseUrl);
        var result = await api.GetHealthAsync();

        Logger.Info($"status={result.Status} app={result.Body.App} commit={result.Body.Commit ?? "<null>"}");
        Assert.Equal(200, result.Status);
        Assert.Equal("healthy", result.Body.Status);
        Assert.Equal("Tedile", result.Body.App);
    }
}
