using GymTracker.Identity.Models;
using GymTracker.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Identity.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Conflict(new { error = result.Error });

        return Ok(new AuthResponse { Token = result.Token!, UserId = result.UserId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(new AuthResponse { Token = result.Token!, UserId = result.UserId });
    }
}
