using System.Security.Claims;
using FitnessAPI.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    protected string CurrentUserEmail =>
        User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    protected IEnumerable<string> CurrentUserRoles =>
        User.FindAll(ClaimTypes.Role).Select(x => x.Value);

    protected bool IsAdmin => CurrentUserRoles.Contains("Admin");

    protected ActionResult<ApiResponseDto<T>> OkResponse<T>(T data, string? message = null)
        => Ok(ApiResponseDto<T>.Ok(data, message));

    protected ActionResult<ApiResponseDto> OkResponse(string? message = null)
        => Ok(ApiResponseDto.Ok(message));

    protected ActionResult<ApiResponseDto<T>> CreatedResponse<T>(T data, string? message = null)
        => StatusCode(201, ApiResponseDto<T>.Ok(data, message));
}
