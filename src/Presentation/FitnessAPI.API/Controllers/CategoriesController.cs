using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Domain.Entities;
using FitnessAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

/// <summary>Exercise categories and muscle groups</summary>
[Tags("Categories")]
public class CategoriesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriesController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <summary>Get all categories</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Exercises.FindAsync(x => false, cancellationToken);
        // Return categories from DB
        return Ok(ApiResponseDto<string>.Ok("Categories endpoint"));
    }

    /// <summary>Create a category (admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return StatusCode(201, ApiResponseDto<object>.Ok(new { category.Id, category.Name }));
    }
}

/// <summary>Muscle groups</summary>
[Tags("MuscleGroups")]
[Route("api/v1/muscle-groups")]
[ApiController]
public class MuscleGroupsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public MuscleGroupsController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <summary>Get all muscle groups</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(ApiResponseDto<string>.Ok("Muscle groups endpoint"));
    }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
}
