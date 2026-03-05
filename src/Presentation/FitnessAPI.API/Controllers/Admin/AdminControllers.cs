using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.DTOs.User;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers.Admin;

/// <summary>Admin - User management</summary>
[Tags("Admin - Users")]
[Route("api/v1/admin/users")]
[Authorize(Roles = "Admin")]
[ApiController]
public class AdminUsersController : BaseController
{
    private readonly IUserService _userService;

    public AdminUsersController(IUserService userService) => _userService = userService;

    /// <summary>Get all users (paginated)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResultDto<UserDto>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(page, pageSize, search, isActive, cancellationToken);
        return Ok(ApiResponseDto<PagedResultDto<UserDto>>.Ok(result));
    }

    /// <summary>Get user by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null) return NotFound(ApiResponseDto.Fail("User not found."));
        return Ok(ApiResponseDto<UserDto>.Ok(user));
    }

    /// <summary>Create a new user</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto), 409)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateUserAsync(dto, cancellationToken);
        return StatusCode(201, ApiResponseDto<UserDto>.Ok(result, "User created successfully."));
    }

    /// <summary>Update a user</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserAsync(id, dto, cancellationToken);
        return Ok(ApiResponseDto<UserDto>.Ok(result, "User updated successfully."));
    }

    /// <summary>Delete a user</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (id == CurrentUserId)
            return BadRequest(ApiResponseDto.Fail("You cannot delete your own account."));
        await _userService.DeleteUserAsync(id, cancellationToken);
        return Ok(ApiResponseDto.Ok("User deleted successfully."));
    }

    /// <summary>Toggle user active status</summary>
    [HttpPost("{id:guid}/toggle-status")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken cancellationToken)
    {
        if (id == CurrentUserId)
            return BadRequest(ApiResponseDto.Fail("You cannot deactivate your own account."));
        var isActive = await _userService.ToggleUserStatusAsync(id, cancellationToken);
        return Ok(ApiResponseDto<bool>.Ok(isActive, isActive ? "User activated." : "User deactivated."));
    }
}

/// <summary>Admin - Dashboard statistics</summary>
[Tags("Admin - Dashboard")]
[Route("api/v1/admin/dashboard")]
[Authorize(Roles = "Admin")]
[ApiController]
public class AdminDashboardController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminDashboardController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <summary>Get dashboard statistics</summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponseDto<DashboardStatsDto>), 200)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var totalUsers = await _unitOfWork.Users.CountAsync(cancellationToken: cancellationToken);
        var activeUsers = await _unitOfWork.Users.CountAsync(x => x.IsActive, cancellationToken);
        var totalWorkoutPlans = await _unitOfWork.WorkoutPlans.CountAsync(cancellationToken: cancellationToken);
        var totalExercises = await _unitOfWork.Exercises.CountAsync(cancellationToken: cancellationToken);
        var pendingExercises = await _unitOfWork.Exercises.CountAsync(x => !x.IsApproved, cancellationToken);
        var totalWorkoutLogs = await _unitOfWork.WorkoutLogs.CountAsync(cancellationToken: cancellationToken);

        var stats = new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = totalUsers - activeUsers,
            TotalWorkoutPlans = totalWorkoutPlans,
            TotalExercises = totalExercises,
            PendingExerciseApprovals = pendingExercises,
            TotalWorkoutLogs = totalWorkoutLogs
        };

        return Ok(ApiResponseDto<DashboardStatsDto>.Ok(stats));
    }

    /// <summary>Get recently registered users</summary>
    [HttpGet("recent-users")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetRecentUsers(CancellationToken cancellationToken)
    {
        var (users, _) = await _unitOfWork.Users.GetPagedAsync(1, 10, null, null, cancellationToken);
        var result = users.Select(u => new { u.Id, u.FullName, u.Email, u.IsActive, u.CreatedAt });
        return Ok(ApiResponseDto<object>.Ok(result));
    }
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalWorkoutPlans { get; set; }
    public int TotalExercises { get; set; }
    public int PendingExerciseApprovals { get; set; }
    public int TotalWorkoutLogs { get; set; }
}
