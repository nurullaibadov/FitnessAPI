using FitnessAPI.Application.Interfaces;
using FitnessAPI.Application.Interfaces.Repositories;
using FitnessAPI.Persistence.Context;
using FitnessAPI.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitnessAPI.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly FitnessDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IWorkoutPlanRepository? _workoutPlans;
    private IExerciseRepository? _exercises;
    private IWorkoutLogRepository? _workoutLogs;
    private IRefreshTokenRepository? _refreshTokens;

    public UnitOfWork(FitnessDbContext context) => _context = context;

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IWorkoutPlanRepository WorkoutPlans => _workoutPlans ??= new WorkoutPlanRepository(_context);
    public IExerciseRepository Exercises => _exercises ??= new ExerciseRepository(_context);
    public IWorkoutLogRepository WorkoutLogs => _workoutLogs ??= new WorkoutLogRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try { await _transaction?.CommitAsync(cancellationToken)!; }
        finally { _transaction?.Dispose(); _transaction = null; }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try { await _transaction?.RollbackAsync(cancellationToken)!; }
        finally { _transaction?.Dispose(); _transaction = null; }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
