using System.Text.Json;

namespace Tedile.Automation.Configuration;

public static class ConfigReader
{
    public static TestSettings ReadConfig()
    {
        var fileSettings = LoadFileSettings();

        return new TestSettings
        {
            BaseUrl = ReadString("TEDILE_BASE_URL", fileSettings.BaseUrl),
            Browser = ReadString("PLAYWRIGHT_BROWSER", fileSettings.Browser),
            Headless = ReadBool("PLAYWRIGHT_HEADLESS", fileSettings.Headless),
            SlowMoMs = ReadFloat("PLAYWRIGHT_SLOWMO_MS", fileSettings.SlowMoMs),
            TimeoutMs = ReadFloat("PLAYWRIGHT_TIMEOUT_MS", fileSettings.TimeoutMs),
            TraceEnabled = ReadBool("PLAYWRIGHT_TRACE", fileSettings.TraceEnabled),
            VideoEnabled = ReadBool("PLAYWRIGHT_VIDEO", fileSettings.VideoEnabled),
            CustomerPhone = Environment.GetEnvironmentVariable("TEDILE_E2E_CUSTOMER_PHONE"),
            CustomerOtp = Environment.GetEnvironmentVariable("TEDILE_E2E_CUSTOMER_OTP")
        };
    }

    private static TestSettings LoadFileSettings()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "testsettings.json");
        if (!File.Exists(path))
        {
            return new TestSettings();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TestSettings>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new TestSettings();
    }

    private static string ReadString(string key, string fallback) =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key))
            ? fallback
            : Environment.GetEnvironmentVariable(key)!.Trim();

    private static bool ReadBool(string key, bool fallback) =>
        bool.TryParse(Environment.GetEnvironmentVariable(key), out var parsed)
            ? parsed
            : fallback;

    private static float ReadFloat(string key, float fallback) =>
        float.TryParse(Environment.GetEnvironmentVariable(key), out var parsed)
            ? parsed
            : fallback;
}
