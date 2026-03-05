using FitnessAPI.Application.DTOs.Auth;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Application.DTOs.User;
using FitnessAPI.Domain.Entities;

namespace FitnessAPI.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task VerifyEmailAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface ITokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
    bool ValidateToken(string token);
}

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string name, string token);
    Task SendPasswordResetAsync(string email, string name, string token);
    Task SendWelcomeEmailAsync(string email, string name);
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
}

public interface IFileService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default);
    bool IsValidFileExtension(string fileName);
    bool IsValidFileSize(long fileSize);
}

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<string> UpdateProfileImageAsync(Guid userId, Stream imageStream, string fileName, CancellationToken cancellationToken = default);
    Task<PagedResultDto<UserDto>> GetAllAsync(int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ToggleUserStatusAsync(Guid id, CancellationToken cancellationToken = default);
}
