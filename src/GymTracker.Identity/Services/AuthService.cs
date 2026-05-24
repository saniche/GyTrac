using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        var token = GenerateToken(user);
        return AuthResult.Success(token, user.Id);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var emailLower = request.Email.ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailLower, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return AuthResult.Failure("Invalid email or password.");

        var token = GenerateToken(user);
        return AuthResult.Success(token, user.Id);
    }

    private string GenerateToken(IdentityUser user)
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
