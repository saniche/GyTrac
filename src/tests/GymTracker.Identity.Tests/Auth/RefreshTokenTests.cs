using System.Net;
using System.Net.Http.Json;
using GymTracker.Identity.Models;

namespace GymTracker.Identity.Tests.Auth;

public sealed class RefreshTokenTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RefreshTokenTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static string UniqueEmail() => $"user_{Guid.NewGuid():N}@test.com";

    /// <summary>Registers a new user and returns the initial auth response (with refresh token).</summary>
    private async Task<AuthResponse> RegisterAndGetTokensAsync()
    {
        var email = UniqueEmail();
        const string password = "Password123!";
        var payload = new { email, password, confirmPassword = password };
        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }

    [Fact]
    public async Task Refresh_WithValidToken_Returns200WithNewTokens()
    {
        var initial = await RegisterAndGetTokensAsync();
        var payload = new { refreshToken = initial.RefreshToken };

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.NotEqual(Guid.Empty, result.UserId);
    }

    [Fact]
    public async Task Refresh_IssuedTokenIsDifferentFromOriginal()
    {
        var initial = await RegisterAndGetTokensAsync();
        var payload = new { refreshToken = initial.RefreshToken };

        var result = await (await _client.PostAsJsonAsync("/api/auth/refresh", payload))
            .Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(result);
        // Token rotation: the new access and refresh tokens must differ from the originals
        Assert.NotEqual(initial.Token, result.Token);
        Assert.NotEqual(initial.RefreshToken, result.RefreshToken);
    }

    [Fact]
    public async Task Refresh_WithRevokedToken_Returns401()
    {
        var initial = await RegisterAndGetTokensAsync();
        var payload = new { refreshToken = initial.RefreshToken };

        // First use — valid
        var first = await _client.PostAsJsonAsync("/api/auth/refresh", payload);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        // Second use of the same (now revoked) token must be rejected
        var second = await _client.PostAsJsonAsync("/api/auth/refresh", payload);
        Assert.Equal(HttpStatusCode.Unauthorized, second.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_Returns401()
    {
        var payload = new { refreshToken = "this-is-not-a-valid-refresh-token" };

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithMissingToken_Returns400()
    {
        var payload = new { };

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_ReturnsDifferentAccessTokenAfterRotation()
    {
        // Verify that the refresh operation actually returns a different access token
        var initial = await RegisterAndGetTokensAsync();
        var payload = new { refreshToken = initial.RefreshToken };

        var refreshed = await (await _client.PostAsJsonAsync("/api/auth/refresh", payload))
            .Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(refreshed);
        // The new access token must not be the same as the previous one
        Assert.NotEqual(initial.Token, refreshed.Token);
    }
}
