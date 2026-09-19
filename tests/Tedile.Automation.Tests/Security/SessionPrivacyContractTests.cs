using System.Text.Json;

namespace Tedile.Automation.Tests.Security;

public sealed class SessionPrivacyContractTests : AuthenticatedCustomerBaseTest
{
    [Test, RequiresCustomerCredentials]
    public async Task Authenticated_session_api_exposes_only_minimal_identity_fields()
    {
        var response = await Page.APIRequest.GetAsync("/api/session");
        Assert.Equal(200, response.Status);

        var body = await response.TextAsync();
        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;

        Assert.True(root.GetProperty("authenticated").GetBoolean());
        Assert.True(root.TryGetProperty("user", out var user));

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "id",
            "name",
            "role",
            "roles"
        };

        foreach (var property in user.EnumerateObject())
        {
            Assert.True(
                allowed.Contains(property.Name),
                $"Unexpected session identity field exposed by /api/session: {property.Name}");
        }

        Assert.True(user.TryGetProperty("id", out _));
        Assert.True(user.TryGetProperty("name", out _));
        Assert.True(user.TryGetProperty("role", out _));
        Assert.True(user.TryGetProperty("roles", out _));

        Assert.DoesNotContain("email", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("phone", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password_hash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("otp", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", body, StringComparison.OrdinalIgnoreCase);
    }
}
