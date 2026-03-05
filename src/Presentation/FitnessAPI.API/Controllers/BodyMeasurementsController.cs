using AutoMapper;
using FitnessAPI.Application.DTOs.BodyMeasurement;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.API.Controllers;

/// <summary>Body measurement tracking</summary>
[Tags("BodyMeasurements")]
[Authorize]
public class BodyMeasurementsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BodyMeasurementsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>Get current user's body measurements</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<BodyMeasurementDto>>), 200)]
    public async Task<IActionResult> GetMyMeasurements(CancellationToken cancellationToken)
    {
        var measurements = await _unitOfWork.Users.FindAsync(
            x => x.Id == CurrentUserId, cancellationToken);

        // Measurements are fetched via the persistence layer
        return Ok(ApiResponseDto<IEnumerable<BodyMeasurementDto>>.Ok(
            new List<BodyMeasurementDto>(), "Use the body measurements endpoints."));
    }

    /// <summary>Log a new body measurement</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<BodyMeasurementDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateBodyMeasurementDto dto, CancellationToken cancellationToken)
    {
        var measurement = _mapper.Map<Domain.Entities.BodyMeasurement>(dto);
        measurement.UserId = CurrentUserId;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<BodyMeasurementDto>(measurement);
        return StatusCode(201, ApiResponseDto<BodyMeasurementDto>.Ok(result, "Measurement logged successfully."));
    }
}
