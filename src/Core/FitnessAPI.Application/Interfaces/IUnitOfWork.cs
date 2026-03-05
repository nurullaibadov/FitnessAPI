using FitnessAPI.Application.Interfaces.Repositories;

namespace FitnessAPI.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IWorkoutPlanRepository WorkoutPlans { get; }
    IExerciseRepository Exercises { get; }
    IWorkoutLogRepository WorkoutLogs { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
