using System.Text.Json.Serialization;

namespace Tedile.Automation.Models;

public sealed class HealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("app")]
    public string App { get; init; } = string.Empty;

    [JsonPropertyName("commit")]
    public string? Commit { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
