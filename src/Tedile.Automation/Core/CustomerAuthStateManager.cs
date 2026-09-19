using System.Text.RegularExpressions;
using Microsoft.Playwright;

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

            // Build the authenticated storage state through Playwright's
            // request client attached to the browser context. This shares the
            // context cookie jar but avoids engine-specific navigation/load
            // races while bootstrapping Chromium, Firefox, and WebKit.
            var bootstrapPage = await context.NewPageAsync();
            var request = bootstrapPage.APIRequest;

            var welcomeResponse = await request.GetAsync("/");
            if (!welcomeResponse.Ok)
            {
                throw new InvalidOperationException(
                    $"Could not open Tedile welcome page while creating customer auth state. HTTP {welcomeResponse.Status}.");
            }

            var csrf = ExtractCsrfToken(await welcomeResponse.TextAsync());

            var loginResponse = await request.PostAsync(
                "/customer/login",
                new APIRequestContextOptions
                {
                    Form = new Dictionary<string, object>
                    {
                        ["csrf_token"] = csrf,
                        ["phone"] = fixture.Settings.CustomerPhone!
                    },
                    MaxRedirects = 0
                });

            if (loginResponse.Status != 302)
            {
                throw new InvalidOperationException(
                    $"Customer OTP challenge could not be started while creating auth state. HTTP {loginResponse.Status}.");
            }

            var otpPageResponse = await request.GetAsync("/otp");
            if (!otpPageResponse.Ok)
            {
                throw new InvalidOperationException(
                    $"OTP page could not be opened while creating auth state. HTTP {otpPageResponse.Status}.");
            }

            csrf = ExtractCsrfToken(await otpPageResponse.TextAsync());

            var verifyResponse = await request.PostAsync(
                "/otp/verify",
                new APIRequestContextOptions
                {
                    Form = new Dictionary<string, object>
                    {
                        ["csrf_token"] = csrf,
                        ["otp"] = fixture.Settings.CustomerOtp!
                    },
                    MaxRedirects = 0
                });

            if (verifyResponse.Status != 302)
            {
                throw new InvalidOperationException(
                    $"Review customer OTP verification failed while creating auth state. HTTP {verifyResponse.Status}.");
            }

            var sessionResponse = await request.GetAsync("/api/session");
            var sessionBody = await sessionResponse.TextAsync();
            if (sessionResponse.Status != 200 ||
                !sessionBody.Contains("\"authenticated\":true", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Customer session was not authenticated after OTP verification. HTTP {sessionResponse.Status}.");
            }

            var authFolder = Path.Combine(Path.GetTempPath(), "tedile-automation-auth");
            Directory.CreateDirectory(authFolder);
            _storageStatePath = Path.Combine(
                authFolder,
                $"customer-storage-state-{Environment.ProcessId}.json");

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

    private static string ExtractCsrfToken(string html)
    {
        var match = Regex.Match(
            html,
            """<meta\s+name=["']csrf-token["']\s+content=["']([^"']+)["']""",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        if (!match.Success)
        {
            throw new InvalidOperationException(
                "Tedile did not return a CSRF token while creating customer auth state.");
        }

        return match.Groups[1].Value;
    }
}
