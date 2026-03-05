using FitnessAPI.Application.DTOs.Auth;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

/// <summary>Authentication endpoints</summary>
[Tags("Auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Login with email and password</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(ApiResponseDto<AuthResponseDto>.Ok(result, "Login successful."));
    }

    /// <summary>Register a new account</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto), 409)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return StatusCode(201, ApiResponseDto<AuthResponseDto>.Ok(result, "Registration successful. Please verify your email."));
    }

    /// <summary>Refresh access token using refresh token</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 401)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, cancellationToken);
        return Ok(ApiResponseDto<AuthResponseDto>.Ok(result));
    }

    /// <summary>Request a password reset email</summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken cancellationToken)
    {
        await _authService.ForgotPasswordAsync(dto, cancellationToken);
        return Ok(ApiResponseDto.Ok("If the email exists, a reset link has been sent."));
    }

    /// <summary>Reset password using token from email</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 400)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken cancellationToken)
    {
        await _authService.ResetPasswordAsync(dto, cancellationToken);
        return Ok(ApiResponseDto.Ok("Password reset successfully."));
    }

    /// <summary>Change password (authenticated users)</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 400)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        await _authService.ChangePasswordAsync(CurrentUserId, dto, cancellationToken);
        return Ok(ApiResponseDto.Ok("Password changed successfully."));
    }

    /// <summary>Verify email address using token</summary>
    [HttpGet("verify-email")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 400)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token, CancellationToken cancellationToken)
    {
        await _authService.VerifyEmailAsync(token, cancellationToken);
        return Ok(ApiResponseDto.Ok("Email verified successfully."));
    }

    /// <summary>Logout and revoke all tokens</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(CurrentUserId, cancellationToken);
        return Ok(ApiResponseDto.Ok("Logged out successfully."));
    }

    /// <summary>Revoke a specific refresh token</summary>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        await _authService.RevokeTokenAsync(dto.RefreshToken, cancellationToken);
        return Ok(ApiResponseDto.Ok("Token revoked."));
    }
}
