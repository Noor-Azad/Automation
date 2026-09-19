namespace Tedile.Automation.Configuration;

public sealed class TestSettings
{
    public string BaseUrl { get; init; } = "https://tedile.in";
    public string Browser { get; init; } = "chromium";
    public bool Headless { get; init; } = true;
    public float SlowMoMs { get; init; }
    public float TimeoutMs { get; init; } = 30_000;
    public bool TraceEnabled { get; init; } = true;
    public bool VideoEnabled { get; init; }
    public string? CustomerPhone { get; init; }
    public string? CustomerOtp { get; init; }

    public bool HasCustomerCredentials =>
        !string.IsNullOrWhiteSpace(CustomerPhone) &&
        !string.IsNullOrWhiteSpace(CustomerOtp);
}
