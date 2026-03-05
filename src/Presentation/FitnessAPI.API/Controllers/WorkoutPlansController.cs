using AutoMapper;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.DTOs.WorkoutPlan;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Domain.Entities;
using FitnessAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

/// <summary>Workout plan management</summary>
[Tags("WorkoutPlans")]
[Authorize]
public class WorkoutPlansController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WorkoutPlansController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>Get all workout plans (paginated)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResultDto<WorkoutPlanDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        var userId = IsAdmin ? (Guid?)null : CurrentUserId;
        var (plans, total) = await _unitOfWork.WorkoutPlans.GetPagedAsync(userId, page, pageSize, search, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<WorkoutPlanDto>>(plans);
        var result = new PagedResultDto<WorkoutPlanDto> { Items = dtos, TotalCount = total, Page = page, PageSize = pageSize };
        return Ok(ApiResponseDto<PagedResultDto<WorkoutPlanDto>>.Ok(result));
    }

    /// <summary>Get public workout plans</summary>
    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<WorkoutPlanDto>>), 200)]
    public async Task<IActionResult> GetPublic(CancellationToken cancellationToken)
    {
        var plans = await _unitOfWork.WorkoutPlans.GetPublicPlansAsync(cancellationToken);
        return Ok(ApiResponseDto<IEnumerable<WorkoutPlanDto>>.Ok(_mapper.Map<IEnumerable<WorkoutPlanDto>>(plans)));
    }

    /// <summary>Get workout plan by ID with all details</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<WorkoutPlanDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.WorkoutPlans.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException("WorkoutPlan", id);

        if (!IsAdmin && plan.UserId != CurrentUserId && !plan.IsPublic)
            throw new UnauthorizedException();

        return Ok(ApiResponseDto<WorkoutPlanDto>.Ok(_mapper.Map<WorkoutPlanDto>(plan)));
    }

    /// <summary>Create a new workout plan</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<WorkoutPlanDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateWorkoutPlanDto dto, CancellationToken cancellationToken)
    {
        var plan = _mapper.Map<WorkoutPlan>(dto);
        plan.UserId = CurrentUserId;

        await _unitOfWork.WorkoutPlans.AddAsync(plan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.WorkoutPlans.GetWithDetailsAsync(plan.Id, cancellationToken);
        return StatusCode(201, ApiResponseDto<WorkoutPlanDto>.Ok(_mapper.Map<WorkoutPlanDto>(created), "Workout plan created successfully."));
    }

    /// <summary>Update a workout plan</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<WorkoutPlanDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 403)]
    [ProducesResponseType(typeof(ApiResponseDto), 404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkoutPlanDto dto, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.WorkoutPlans.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("WorkoutPlan", id);

        if (!IsAdmin && plan.UserId != CurrentUserId)
            throw new UnauthorizedException("You can only update your own workout plans.");

        if (dto.Name != null) plan.Name = dto.Name;
        if (dto.Description != null) plan.Description = dto.Description;
        if (dto.DifficultyLevel.HasValue) plan.DifficultyLevel = dto.DifficultyLevel.Value;
        if (dto.GoalType.HasValue) plan.GoalType = dto.GoalType.Value;
        if (dto.IsPublic.HasValue) plan.IsPublic = dto.IsPublic.Value;
        if (dto.IsActive.HasValue) plan.IsActive = dto.IsActive.Value;

        _unitOfWork.WorkoutPlans.Update(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponseDto<WorkoutPlanDto>.Ok(_mapper.Map<WorkoutPlanDto>(plan), "Updated successfully."));
    }

    /// <summary>Delete a workout plan</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 403)]
    [ProducesResponseType(typeof(ApiResponseDto), 404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.WorkoutPlans.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("WorkoutPlan", id);

        if (!IsAdmin && plan.UserId != CurrentUserId)
            throw new UnauthorizedException("You can only delete your own workout plans.");

        _unitOfWork.WorkoutPlans.Remove(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponseDto.Ok("Workout plan deleted successfully."));
    }
}
