using System.Net;
using System.Net.Http.Json;
using GymTracker.Identity.Models;

namespace GymTracker.Identity.Tests.Auth;

public sealed class LoginTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static string UniqueEmail() => $"user_{Guid.NewGuid():N}@test.com";

    /// <summary>Registers a fresh user and returns its credentials.</summary>
    private async Task<(string Email, string Password)> CreateUserAsync()
    {
        var email = UniqueEmail();
        const string password = "Password123!";
        var payload = new { email, password, confirmPassword = password };
        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);
        response.EnsureSuccessStatusCode();
        return (email, password);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithTokens()
    {
        var (email, password) = await CreateUserAsync();
        var payload = new { email, password };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.NotEqual(Guid.Empty, result.UserId);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var (email, _) = await CreateUserAsync();
        var payload = new { email, password = "WrongPassword!" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns401()
    {
        var payload = new { email = "ghost@test.com", password = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithMissingPassword_Returns400()
    {
        var (email, _) = await CreateUserAsync();
        var payload = new { email };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidEmailFormat_Returns400()
    {
        var payload = new { email = "not-an-email", password = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_IsCaseInsensitiveForEmail_Returns200()
    {
        var (email, password) = await CreateUserAsync();
        // Log in with the email in upper case
        var payload = new { email = email.ToUpper(), password };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_EachSuccessfulLogin_IssuesNewRefreshToken()
    {
        var (email, password) = await CreateUserAsync();
        var payload = new { email, password };

        var first = await (await _client.PostAsJsonAsync("/api/auth/login", payload))
            .Content.ReadFromJsonAsync<AuthResponse>();
        var second = await (await _client.PostAsJsonAsync("/api/auth/login", payload))
            .Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotEqual(first.RefreshToken, second.RefreshToken);
    }
}
