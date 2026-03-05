using AutoMapper;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.DTOs.WorkoutLog;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Domain.Entities;
using FitnessAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

/// <summary>Workout log tracking</summary>
[Tags("WorkoutLogs")]
[Authorize]
public class WorkoutLogsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WorkoutLogsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>Get current user's workout logs</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<WorkoutLogDto>>), 200)]
    public async Task<IActionResult> GetMyLogs(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var logs = await _unitOfWork.WorkoutLogs.GetUserLogsAsync(CurrentUserId, from, to, cancellationToken);
        return Ok(ApiResponseDto<IEnumerable<WorkoutLogDto>>.Ok(_mapper.Map<IEnumerable<WorkoutLogDto>>(logs)));
    }

    /// <summary>Get a specific workout log with details</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<WorkoutLogDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var log = await _unitOfWork.WorkoutLogs.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException("WorkoutLog", id);

        if (!IsAdmin && log.UserId != CurrentUserId)
            throw new UnauthorizedException();

        return Ok(ApiResponseDto<WorkoutLogDto>.Ok(_mapper.Map<WorkoutLogDto>(log)));
    }

    /// <summary>Log a completed workout session</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<WorkoutLogDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateWorkoutLogDto dto, CancellationToken cancellationToken)
    {
        var log = new WorkoutLog
        {
            UserId = CurrentUserId,
            WorkoutPlanId = dto.WorkoutPlanId,
            WorkoutDayId = dto.WorkoutDayId,
            StartedAt = dto.StartedAt,
            CompletedAt = dto.CompletedAt,
            TotalDurationMinutes = dto.TotalDurationMinutes,
            CaloriesBurned = dto.CaloriesBurned,
            Notes = dto.Notes,
            OverallRating = dto.OverallRating,
            IsCompleted = dto.CompletedAt.HasValue,
            WorkoutLogDetails = dto.Details.Select(d => new WorkoutLogDetail
            {
                ExerciseId = d.ExerciseId,
                ActualSets = d.ActualSets,
                ActualReps = d.ActualReps,
                ActualWeightKg = d.ActualWeightKg,
                ActualDurationSeconds = d.ActualDurationSeconds,
                Notes = d.Notes,
                OrderIndex = d.OrderIndex
            }).ToList()
        };

        await _unitOfWork.WorkoutLogs.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.WorkoutLogs.GetWithDetailsAsync(log.Id, cancellationToken);
        return StatusCode(201, ApiResponseDto<WorkoutLogDto>.Ok(
            _mapper.Map<WorkoutLogDto>(created), "Workout logged successfully."));
    }

    /// <summary>Delete a workout log</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var log = await _unitOfWork.WorkoutLogs.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("WorkoutLog", id);

        if (!IsAdmin && log.UserId != CurrentUserId)
            throw new UnauthorizedException();

        _unitOfWork.WorkoutLogs.Remove(log);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponseDto.Ok("Workout log deleted."));
    }
}
