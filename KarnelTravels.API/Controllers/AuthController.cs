using KarnelTravels.API.DTOs;
using KarnelTravels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("admin/login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> AdminLogin([FromBody] LoginRequest request)
    {
        var result = await _authService.AdminLoginAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request.Email);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> Logout()
    {
        var result = await _authService.LogoutAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _authService.GetCurrentUserAsync(userId);
        return Ok(result);
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _authService.ChangePasswordAsync(userId, request);
        return Ok(result);
    }
}
