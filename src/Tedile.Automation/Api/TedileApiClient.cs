using System.Text.Json;
using Microsoft.Playwright;
using Tedile.Automation.Models;

namespace Tedile.Automation.Api;

public sealed class TedileApiClient : IAsyncDisposable
{
    private readonly IAPIRequestContext _request;

    private TedileApiClient(IAPIRequestContext request) => _request = request;

    public static async Task<TedileApiClient> CreateAsync(IPlaywright playwright, string baseUrl)
    {
        var request = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = baseUrl,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept"] = "application/json"
            }
        });

        return new TedileApiClient(request);
    }

    public async Task<(int Status, HealthResponse Body)> GetHealthAsync()
    {
        var response = await _request.GetAsync("/health");
        var json = await response.TextAsync();
        var body = JsonSerializer.Deserialize<HealthResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Health response could not be deserialized.");

        return (response.Status, body);
    }

    public async ValueTask DisposeAsync() => await _request.DisposeAsync();
}
