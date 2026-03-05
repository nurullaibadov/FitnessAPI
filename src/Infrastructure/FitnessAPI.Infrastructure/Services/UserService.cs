using AutoMapper;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.DTOs.User;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Application.Interfaces.Services;
using FitnessAPI.Domain.Entities;
using FitnessAPI.Domain.Exceptions;

namespace FitnessAPI.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetWithRolesAsync(id, cancellationToken);
        if (user == null) return null;
        var dto = _mapper.Map<UserDto>(user);
        dto.Roles = user.UserRoles.Select(x => x.Role.Name);
        return dto;
    }

    public async Task<UserProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetWithRolesAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        var dto = _mapper.Map<UserProfileDto>(user);
        dto.Roles = user.UserRoles.Select(x => x.Role.Name);

        var progress = await _unitOfWork.WorkoutLogs.GetUserProgressAsync(userId, cancellationToken);
        if (progress != null)
        {
            dto.TotalWorkouts = progress.TotalWorkouts;
            dto.TotalCaloriesBurned = progress.TotalCaloriesBurned;
            dto.CurrentStreak = progress.CurrentStreak;
            dto.TotalWorkoutMinutes = progress.TotalWorkoutMinutes;
        }

        var lastLog = await _unitOfWork.WorkoutLogs.FirstOrDefaultAsync(x => x.UserId == userId && x.IsCompleted, cancellationToken);
        dto.LastWorkoutDate = lastLog?.CompletedAt;

        return dto;
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetWithRolesAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
        if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth;
        if (dto.Gender.HasValue) user.Gender = dto.Gender.Value;
        if (dto.GoalType.HasValue) user.GoalType = dto.GoalType.Value;
        if (dto.Height.HasValue) user.Height = dto.Height;
        if (dto.Weight.HasValue) user.Weight = dto.Weight;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<UserDto>(user);
        result.Roles = user.UserRoles.Select(x => x.Role.Name);
        return result;
    }

    public async Task<string> UpdateProfileImageAsync(Guid userId, Stream imageStream, string fileName, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        if (!_fileService.IsValidFileExtension(fileName))
            throw new BusinessException("Invalid file type. Only jpg, jpeg, png, webp are allowed.");

        // Delete old image if exists
        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            await _fileService.DeleteAsync(user.ProfileImageUrl, cancellationToken);

        var imageUrl = await _fileService.UploadAsync(imageStream, fileName, "profiles", cancellationToken);
        user.ProfileImageUrl = imageUrl;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return imageUrl;
    }

    public async Task<PagedResultDto<UserDto>> GetAllAsync(int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken = default)
    {
        var (users, totalCount) = await _unitOfWork.Users.GetPagedAsync(page, pageSize, search, isActive, cancellationToken);
        var dtos = users.Select(u =>
        {
            var dto = _mapper.Map<UserDto>(u);
            dto.Roles = u.UserRoles.Select(x => x.Role.Name);
            return dto;
        });

        return new PagedResultDto<UserDto> { Items = dtos, TotalCount = totalCount, Page = page, PageSize = pageSize };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(dto.Email, cancellationToken))
            throw new ConflictException("Email address is already in use.");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.Email = dto.Email.ToLower();
        user.IsEmailVerified = true;

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<UserDto>(user);
        result.Roles = new[] { dto.Role };
        return result;
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
        => await UpdateProfileAsync(id, dto, cancellationToken);

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User", id);
        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ToggleUserStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User", id);
        user.IsActive = !user.IsActive;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return user.IsActive;
    }
}
