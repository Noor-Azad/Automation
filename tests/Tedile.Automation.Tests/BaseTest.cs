using Microsoft.Playwright;
using Tedile.Automation.Core;
using Tedile.Automation.Utilities;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests;

public abstract class BaseTest : IAsyncLifetime
{
    private readonly string _testScopeName;

    protected BaseTest(PlaywrightFixture fixture, ITestOutputHelper output)
    {
        Fixture = fixture;
        Output = output;
        Logger = new TestLogger(output.WriteLine);
        _testScopeName = GetType().Name;
    }

    protected PlaywrightFixture Fixture { get; }
    protected ITestOutputHelper Output { get; }
    protected TestLogger Logger { get; }
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    protected virtual bool CaptureTrace => Fixture.Settings.TraceEnabled;

    protected virtual Task<IBrowserContext> CreateContextAsync() =>
        Fixture.CreateContextAsync();

    public async Task InitializeAsync()
    {
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
