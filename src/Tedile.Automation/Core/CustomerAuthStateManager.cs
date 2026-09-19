using Microsoft.Playwright;
using Tedile.Automation.Pages;

namespace Tedile.Automation.Core;

public static class CustomerAuthStateManager
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static string? _storageStatePath;

    public static async Task<string> GetOrCreateAsync(PlaywrightFixture fixture)
    {
        if (!fixture.Settings.HasCustomerCredentials)
        {
            throw new InvalidOperationException(
                "TEDILE_E2E_CUSTOMER_PHONE and TEDILE_E2E_CUSTOMER_OTP are required for authenticated customer tests.");
        }

        if (!string.IsNullOrWhiteSpace(_storageStatePath) && File.Exists(_storageStatePath))
        {
            return _storageStatePath;
        }

        await Gate.WaitAsync();
        try
        {
            if (!string.IsNullOrWhiteSpace(_storageStatePath) && File.Exists(_storageStatePath))
            {
                return _storageStatePath;
            }

            await fixture.InitializeAsync();
            await using var context = await fixture.CreateContextAsync();
            var page = await context.NewPageAsync();

            var welcome = new WelcomePage(page);
            var otp = new OtpPage(page);

            await welcome.OpenAsync();
            await welcome.RequestCustomerOtpAsync(fixture.Settings.CustomerPhone!);
            await page.WaitForURLAsync("**/otp");
            await otp.VerifyAsync(fixture.Settings.CustomerOtp!);
            await page.WaitForURLAsync("**/customer/dashboard");

            var authFolder = Path.Combine(ArtifactPaths.EnsureRoot(), "auth");
            Directory.CreateDirectory(authFolder);
            _storageStatePath = Path.Combine(authFolder, "customer-storage-state.json");

            await context.StorageStateAsync(new BrowserContextStorageStateOptions
            {
                Path = _storageStatePath
            });

            return _storageStatePath;
        }
        finally
        {
            Gate.Release();
        }
    }
}
