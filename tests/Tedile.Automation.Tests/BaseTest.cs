using Microsoft.Playwright;
using Tedile.Automation.Core;
using Tedile.Automation.Utilities;

namespace Tedile.Automation.Tests;

internal static class NUnitTestRuntime
{
    internal static PlaywrightFixture Fixture { get; } = new();
}

public abstract class BaseTest
{
    private string _testScopeName = string.Empty;

    protected PlaywrightFixture Fixture => NUnitTestRuntime.Fixture;
    protected TestLogger Logger { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    protected virtual bool CaptureTrace => Fixture.Settings.TraceEnabled;

    protected virtual Task<IBrowserContext> CreateContextAsync() =>
        Fixture.CreateContextAsync();

    [SetUp]
    public async Task InitializeAsync()
    {
        Logger = new TestLogger(TestContext.Progress.WriteLine);
        _testScopeName = $"{GetType().Name}-{TestContext.CurrentContext.Test.Name}";
        Context = await CreateContextAsync();

        if (CaptureTrace)
        {
            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        Page = await Context.NewPageAsync();
        Page.Console += (_, message) => Logger.Info($"browser:{message.Type} {message.Text}");
        Page.PageError += (_, message) => Logger.Warn($"page-error: {message}");
    }

    [TearDown]
    public async Task DisposeAsync()
    {
        if (CaptureTrace)
        {
            var tracePath = ArtifactPaths.Trace(_testScopeName);
            await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            Logger.Info($"Trace: {tracePath}");
        }

        await Context.CloseAsync();
    }

    protected async Task<string> CaptureAsync(string name)
    {
        var path = ArtifactPaths.Screenshot(name);
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = true
        });
        Logger.Info($"Screenshot: {path}");
        return path;
    }
}
