using FitnessAPI.Application.Interfaces.Repositories;
using FitnessAPI.Domain.Entities;
using FitnessAPI.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FitnessAPI.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(FitnessDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email.ToLower(), cancellationToken);

    public async Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(x => x.EmailVerificationToken == token, cancellationToken);

    public async Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(x => x.PasswordResetToken == token &&
            x.PasswordResetTokenExpiry > DateTime.UtcNow, cancellationToken);

    public async Task<User?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbSet.Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user?.UserRoles.Select(x => x.Role.Name) ?? Enumerable.Empty<string>();
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(x => x.Email == email.ToLower(), cancellationToken);

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(x => x.UserRoles).ThenInclude(x => x.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.FirstName.Contains(search) || x.LastName.Contains(search) || x.Email.Contains(search));

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var users = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (users, totalCount);
    }
}

public class WorkoutPlanRepository : GenericRepository<WorkoutPlan>, IWorkoutPlanRepository
{
    public WorkoutPlanRepository(FitnessDbContext context) : base(context) { }

    public async Task<WorkoutPlan?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(x => x.User)
            .Include(x => x.WorkoutDays.OrderBy(d => d.DayNumber))
                .ThenInclude(d => d.WorkoutDayExercises.OrderBy(e => e.OrderIndex))
                    .ThenInclude(e => e.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IEnumerable<WorkoutPlan> Plans, int TotalCount)> GetPagedAsync(Guid? userId, int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(x => x.User).AsQueryable();

        if (userId.HasValue) query = query.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));

        var totalCount = await query.CountAsync(cancellationToken);
        var plans = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (plans, totalCount);
    }

    public async Task<IEnumerable<WorkoutPlan>> GetPublicPlansAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Include(x => x.User).Where(x => x.IsPublic && x.IsActive)
            .OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
}

public class ExerciseRepository : GenericRepository<Exercise>, IExerciseRepository
{
    public ExerciseRepository(FitnessDbContext context) : base(context) { }

    public async Task<Exercise?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(x => x.Category).Include(x => x.MuscleGroup)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IEnumerable<Exercise> Exercises, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search, Guid? categoryId, bool? isApproved, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(x => x.Category).Include(x => x.MuscleGroup).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));
        if (categoryId.HasValue) query = query.Where(x => x.CategoryId == categoryId.Value);
        if (isApproved.HasValue) query = query.Where(x => x.IsApproved == isApproved.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var exercises = await query.OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (exercises, totalCount);
    }
}

public class WorkoutLogRepository : GenericRepository<WorkoutLog>, IWorkoutLogRepository
{
    public WorkoutLogRepository(FitnessDbContext context) : base(context) { }

    public async Task<WorkoutLog?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(x => x.WorkoutPlan)
            .Include(x => x.WorkoutDay)
            .Include(x => x.WorkoutLogDetails).ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<WorkoutLog>> GetUserLogsAsync(Guid userId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(x => x.UserId == userId).AsQueryable();
        if (from.HasValue) query = query.Where(x => x.StartedAt >= from.Value);
        if (to.HasValue) query = query.Where(x => x.StartedAt <= to.Value);
        return await query.OrderByDescending(x => x.StartedAt).ToListAsync(cancellationToken);
    }

    public async Task<UserProgress?> GetUserProgressAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.UserProgresses.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
}

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(FitnessDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => await _dbSet.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbSet.Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedReason = "User logged out";
        }
    }
}
