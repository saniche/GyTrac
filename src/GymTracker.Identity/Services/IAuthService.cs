using GymTracker.Identity.Models;

namespace GymTracker.Identity.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public sealed class AuthResult
{
    public bool IsSuccess { get; private set; }
    public string? Token { get; private set; }
    public Guid UserId { get; private set; }
    public string? Error { get; private set; }

    public static AuthResult Success(string token, Guid userId) =>
        new() { IsSuccess = true, Token = token, UserId = userId };

    public static AuthResult Failure(string error) =>
        new() { IsSuccess = false, Error = error };
}
