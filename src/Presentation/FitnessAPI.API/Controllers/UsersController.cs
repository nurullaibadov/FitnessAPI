using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.DTOs.User;
using FitnessAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

/// <summary>User profile management</summary>
[Tags("Users")]
[Authorize]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    /// <summary>Get current user's profile</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponseDto<UserProfileDto>), 200)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await _userService.GetProfileAsync(CurrentUserId, cancellationToken);
        return Ok(ApiResponseDto<UserProfileDto>.Ok(result));
    }

    /// <summary>Update current user's profile</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateProfileAsync(CurrentUserId, dto, cancellationToken);
        return Ok(ApiResponseDto<UserDto>.Ok(result, "Profile updated successfully."));
    }

    /// <summary>Upload profile image</summary>
    [HttpPost("me/profile-image")]
    [ProducesResponseType(typeof(ApiResponseDto<string>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 400)]
    public async Task<IActionResult> UploadProfileImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponseDto.Fail("No file provided."));

        if (!_userService.GetType().Name.Contains("UserService"))
            return BadRequest(ApiResponseDto.Fail("Service error."));

        using var stream = file.OpenReadStream();
        var imageUrl = await _userService.UpdateProfileImageAsync(CurrentUserId, stream, file.FileName, cancellationToken);
        return Ok(ApiResponseDto<string>.Ok(imageUrl, "Profile image updated successfully."));
    }

    /// <summary>Get user by ID (admin only)</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound(ApiResponseDto.Fail("User not found."));
        return Ok(ApiResponseDto<UserDto>.Ok(result));
    }
}
