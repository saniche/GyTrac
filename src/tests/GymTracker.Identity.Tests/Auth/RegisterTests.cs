using System.Net;
using System.Net.Http.Json;
using GymTracker.Identity.Models;

namespace GymTracker.Identity.Tests.Auth;

public sealed class RegisterTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegisterTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static string UniqueEmail() => $"user_{Guid.NewGuid():N}@test.com";

    [Fact]
    public async Task Register_WithValidRequest_Returns200WithTokens()
    {
        var email = UniqueEmail();
        var payload = new { email, password = "Password123!", confirmPassword = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.NotEqual(Guid.Empty, result.UserId);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var email = UniqueEmail();
        var payload = new { email, password = "Password123!", confirmPassword = "Password123!" };

        // First registration succeeds
        await _client.PostAsJsonAsync("/api/auth/register", payload);

        // Second registration with the same email must be rejected
        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidEmailFormat_Returns400()
    {
        var payload = new { email = "not-an-email", password = "Password123!", confirmPassword = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithPasswordMismatch_Returns400()
    {
        var payload = new { email = UniqueEmail(), password = "Password123!", confirmPassword = "Different456!" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithShortPassword_Returns400()
    {
        // MinLength(8) validation should reject passwords shorter than 8 characters
        var payload = new { email = UniqueEmail(), password = "Pass1!", confirmPassword = "Pass1!" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithMissingEmail_Returns400()
    {
        var payload = new { password = "Password123!", confirmPassword = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_EmailIsCaseInsensitive_SecondRegistrationReturns409()
    {
        var email = UniqueEmail();
        var payload1 = new { email = email.ToUpper(), password = "Password123!", confirmPassword = "Password123!" };
        var payload2 = new { email = email.ToLower(), password = "Password123!", confirmPassword = "Password123!" };

        await _client.PostAsJsonAsync("/api/auth/register", payload1);
        var response = await _client.PostAsJsonAsync("/api/auth/register", payload2);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
