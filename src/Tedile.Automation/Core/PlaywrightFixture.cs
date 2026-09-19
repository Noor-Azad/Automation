using Microsoft.Playwright;
using Tedile.Automation.Configuration;

namespace Tedile.Automation.Core;

public sealed class PlaywrightFixture : IDisposable, IAsyncDisposable
{
    private readonly object _sync = new();
    private Task? _initializeTask;
    private bool _disposed;

    public TestSettings Settings { get; } = ConfigReader.ReadConfig();
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    public Task InitializeAsync()
    {
        lock (_sync)
        {
            _initializeTask ??= InitializeCoreAsync();
            return _initializeTask;
        }
    }

    private async Task InitializeCoreAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browserType = Settings.Browser.Trim().ToLowerInvariant() switch
        {
            "firefox" => Playwright.Firefox,
            "webkit" => Playwright.Webkit,
            _ => Playwright.Chromium
        };

        Browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Settings.Headless,
            SlowMo = Settings.SlowMoMs
        });
    }

    public async Task<IBrowserContext> CreateContextAsync()
    {
        await InitializeAsync();

        var artifacts = ArtifactPaths.EnsureRoot();
        var options = new BrowserNewContextOptions
        {
            BaseURL = Settings.BaseUrl,
            Locale = "en-IN",
            TimezoneId = "Asia/Kolkata",
            IgnoreHTTPSErrors = false,
            ViewportSize = new ViewportSize { Width = 1440, Height = 1000 }
        };

        if (Settings.VideoEnabled)
        {
            options.RecordVideoDir = Path.Combine(artifacts, "videos");
            Directory.CreateDirectory(options.RecordVideoDir);
        }

        var context = await Browser.NewContextAsync(options);
        context.SetDefaultTimeout(Settings.TimeoutMs);
        context.SetDefaultNavigationTimeout(Settings.TimeoutMs);
        return context;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_initializeTask is not null)
        {
            _initializeTask.GetAwaiter().GetResult();
            Browser?.CloseAsync().GetAwaiter().GetResult();
        }

        Playwright?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_initializeTask is not null)
        {
            await _initializeTask;
            if (Browser is not null)
            {
                await Browser.CloseAsync();
            }
        }

        Playwright?.Dispose();
    }
}
