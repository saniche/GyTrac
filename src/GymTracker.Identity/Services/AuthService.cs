using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GymTracker.Identity.Data;
using GymTracker.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GymTracker.Identity.Services;

public sealed class AuthService : IAuthService
{
    private readonly IdentityDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(IdentityDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailLower = request.Email.ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == emailLower, cancellationToken))
            return AuthResult.Failure("An account with this email already exists.");

        var user = new IdentityUser
        {
            Id = Guid.NewGuid(),
            Email = emailLower,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, cancellationToken);
        return AuthResult.Success(accessToken, refreshToken, user.Id);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var emailLower = request.Email.ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailLower, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return AuthResult.Failure("Invalid email or password.");

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, cancellationToken);
        return AuthResult.Success(accessToken, refreshToken, user.Id);
    }

    private string GetSalt()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var salt = jwtSettings["TokenSalt"];
        if (!string.IsNullOrEmpty(salt))
            return salt;

        var secret = jwtSettings["Secret"];
        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JwtSettings:TokenSalt (or JwtSettings:Secret) is not configured.");

        return secret;
    }

    private string HashToken(string token)
    {
        var salt = GetSalt();
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token + salt)));
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = HashToken(refreshToken);

        var stored = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenHash, cancellationToken);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow)
            return AuthResult.Failure("Invalid or expired refresh token.");

        // Rotate: revoke the current token and issue a new pair
        stored.IsRevoked = true;

        var accessToken = GenerateAccessToken(stored.User);
        var newRefreshToken = await CreateRefreshTokenAsync(stored.UserId, cancellationToken);
        return AuthResult.Success(accessToken, newRefreshToken, stored.UserId);
    }

    private async Task<string> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshExpiryDays = int.TryParse(jwtSettings["RefreshExpiresInDays"], out var days) ? days : 30;

        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var rawToken = Convert.ToBase64String(tokenBytes);

        var tokenHash = HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = tokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays),
            IsRevoked = false
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(cancellationToken);

        return rawToken;
    }

    private string GenerateAccessToken(IdentityUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");
        var issuer = jwtSettings["Issuer"] ?? "GymTracker.Identity";
        var audience = jwtSettings["Audience"] ?? "GymTracker.Api";
        var expiresInMinutes = int.TryParse(jwtSettings["ExpiresInMinutes"], out var mins) ? mins : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

