using AutoMapper;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.DTOs.Exercise;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Application.Interfaces.Services;
using FitnessAPI.Domain.Entities;
using FitnessAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

/// <summary>Exercise library management</summary>
[Tags("Exercises")]
public class ExercisesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public ExercisesController(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
    }

    /// <summary>Get all exercises (paginated, filterable)</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResultDto<ExerciseDto>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var isAdmin = User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");
        var (exercises, total) = await _unitOfWork.Exercises.GetPagedAsync(page, pageSize, search, categoryId, isAdmin ? null : true, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<ExerciseDto>>(exercises);
        return Ok(ApiResponseDto<PagedResultDto<ExerciseDto>>.Ok(
            new PagedResultDto<ExerciseDto> { Items = dtos, TotalCount = total, Page = page, PageSize = pageSize }
        ));
    }

    /// <summary>Get exercise by ID</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<ExerciseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto), 404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var exercise = await _unitOfWork.Exercises.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException("Exercise", id);
        return Ok(ApiResponseDto<ExerciseDto>.Ok(_mapper.Map<ExerciseDto>(exercise)));
    }

    /// <summary>Create a new exercise</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<ExerciseDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateExerciseDto dto, CancellationToken cancellationToken)
    {
        var exercise = _mapper.Map<Exercise>(dto);
        exercise.CreatedByUserId = CurrentUserId;
        exercise.IsApproved = IsAdmin;

        await _unitOfWork.Exercises.AddAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Exercises.GetWithDetailsAsync(exercise.Id, cancellationToken);
        return StatusCode(201, ApiResponseDto<ExerciseDto>.Ok(_mapper.Map<ExerciseDto>(created),
            IsAdmin ? "Exercise created and approved." : "Exercise submitted for review."));
    }

    /// <summary>Update an exercise (admin only)</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto<ExerciseDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExerciseDto dto, CancellationToken cancellationToken)
    {
        var exercise = await _unitOfWork.Exercises.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException("Exercise", id);

        if (dto.Name != null) exercise.Name = dto.Name;
        if (dto.Description != null) exercise.Description = dto.Description;
        if (dto.Instructions != null) exercise.Instructions = dto.Instructions;
        if (dto.CategoryId.HasValue) exercise.CategoryId = dto.CategoryId.Value;
        if (dto.MuscleGroupId.HasValue) exercise.MuscleGroupId = dto.MuscleGroupId;
        if (dto.DifficultyLevel.HasValue) exercise.DifficultyLevel = dto.DifficultyLevel.Value;
        if (dto.ExerciseType.HasValue) exercise.ExerciseType = dto.ExerciseType.Value;
        if (dto.VideoUrl != null) exercise.VideoUrl = dto.VideoUrl;
        if (dto.CaloriePerMinute.HasValue) exercise.CaloriePerMinute = dto.CaloriePerMinute;
        if (dto.IsApproved.HasValue) exercise.IsApproved = dto.IsApproved.Value;

        _unitOfWork.Exercises.Update(exercise);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponseDto<ExerciseDto>.Ok(_mapper.Map<ExerciseDto>(exercise)));
    }

    /// <summary>Upload exercise image (admin only)</summary>
    [HttpPost("{id:guid}/image")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto<string>), 200)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Exercise", id);

        if (!_fileService.IsValidFileExtension(file.FileName))
            return BadRequest(ApiResponseDto.Fail("Invalid file type."));

        if (!string.IsNullOrEmpty(exercise.ImageUrl))
            await _fileService.DeleteAsync(exercise.ImageUrl, cancellationToken);

        using var stream = file.OpenReadStream();
        exercise.ImageUrl = await _fileService.UploadAsync(stream, file.FileName, "exercises", cancellationToken);
        _unitOfWork.Exercises.Update(exercise);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponseDto<string>.Ok(exercise.ImageUrl));
    }

    /// <summary>Approve an exercise (admin only)</summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Exercise", id);
        exercise.IsApproved = true;
        _unitOfWork.Exercises.Update(exercise);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponseDto.Ok("Exercise approved."));
    }

    /// <summary>Delete an exercise (admin only)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Exercise", id);
        _unitOfWork.Exercises.Remove(exercise);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponseDto.Ok("Exercise deleted."));
    }
}
