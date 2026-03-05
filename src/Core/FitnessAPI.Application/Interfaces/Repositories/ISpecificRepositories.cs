using FitnessAPI.Domain.Entities;

namespace FitnessAPI.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<User?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken = default);
}

public interface IWorkoutPlanRepository : IGenericRepository<WorkoutPlan>
{
    Task<WorkoutPlan?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<WorkoutPlan> Plans, int TotalCount)> GetPagedAsync(Guid? userId, int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkoutPlan>> GetPublicPlansAsync(CancellationToken cancellationToken = default);
}

public interface IExerciseRepository : IGenericRepository<Exercise>
{
    Task<Exercise?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Exercise> Exercises, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, Guid? categoryId, bool? isApproved, CancellationToken cancellationToken = default);
}

public interface IWorkoutLogRepository : IGenericRepository<WorkoutLog>
{
    Task<WorkoutLog?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkoutLog>> GetUserLogsAsync(Guid userId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<UserProgress?> GetUserProgressAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
