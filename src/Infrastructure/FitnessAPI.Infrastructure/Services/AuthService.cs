using FitnessAPI.Application.DTOs.Auth;
using FitnessAPI.Application.Interfaces;
using FitnessAPI.Application.Interfaces.Services;
using FitnessAPI.Domain.Entities;
using FitnessAPI.Domain.Enums;
using FitnessAPI.Domain.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FitnessAPI.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService,
        IEmailService emailService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email.ToLower(), cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Your account has been deactivated.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var roles = user.UserRoles.Select(x => x.Role.Name).ToList();
        return await GenerateAuthResponseAsync(user, roles, dto.RememberMe, cancellationToken);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(dto.Email, cancellationToken))
            throw new ConflictException("Email address is already in use.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                EmailVerificationToken = Guid.NewGuid().ToString(),
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1),
                IsActive = true,
                IsEmailVerified = false,
                GoalType = GoalType.GeneralFitness
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Add default User role (seeded with fixed ID)
            var userRoleEntity = new UserRole
            {
                UserId = user.Id,
                RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            };
            // UserRole is added via DbContext directly through persistence
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            try
            {
                await _emailService.SendEmailVerificationAsync(user.Email, user.FullName, user.EmailVerificationToken);
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);
            }
            catch { /* Email failure should not block registration */ }

            var roles = new List<string> { "User" };
            return await GenerateAuthResponseAsync(user, roles, false, cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!token.IsActive)
            throw new UnauthorizedException("Refresh token has expired or been revoked.");

        var user = token.User;
        var roles = user.UserRoles.Select(x => x.Role.Name).ToList();

        token.IsRevoked = true;
        token.RevokedReason = "Replaced by new token";
        _unitOfWork.RefreshTokens.Update(token);

        return await GenerateAuthResponseAsync(user, roles, false, cancellationToken);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email.ToLower(), cancellationToken);
        if (user == null) return;

        user.PasswordResetToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(2);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try { await _emailService.SendPasswordResetAsync(user.Email, user.FullName, user.PasswordResetToken); }
        catch { }
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(dto.Token, cancellationToken)
            ?? throw new BusinessException("Invalid or expired password reset token.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        _unitOfWork.Users.Update(user);

        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new BusinessException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailVerificationTokenAsync(token, cancellationToken)
            ?? throw new BusinessException("Invalid or expired verification token.");

        if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            throw new BusinessException("Email verification token has expired.");

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken)
            ?? throw new NotFoundException("Refresh token not found.");

        token.IsRevoked = true;
        token.RevokedReason = "Manually revoked";
        _unitOfWork.RefreshTokens.Update(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(
        User user, List<string> roles, bool rememberMe, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshTokenString = _tokenService.GenerateRefreshToken();

        var refreshTokenDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "30");
        if (!rememberMe) refreshTokenDays = 1;

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "60"));

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            AccessTokenExpiry = accessTokenExpiry,
            RefreshTokenExpiry = refreshToken.ExpiryDate,
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                Roles = roles,
                IsEmailVerified = user.IsEmailVerified
            }
        };
    }
}
