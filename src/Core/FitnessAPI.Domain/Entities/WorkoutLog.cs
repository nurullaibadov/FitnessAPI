using FitnessAPI.Domain.Entities.Common;

namespace FitnessAPI.Domain.Entities;

public class WorkoutLog : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? WorkoutPlanId { get; set; }
    public WorkoutPlan? WorkoutPlan { get; set; }

    public Guid? WorkoutDayId { get; set; }
    public WorkoutDay? WorkoutDay { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalDurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int OverallRating { get; set; }  // 1-5

    public ICollection<WorkoutLogDetail> WorkoutLogDetails { get; set; } = new List<WorkoutLogDetail>();
}

public class WorkoutLogDetail : BaseEntity
{
    public Guid WorkoutLogId { get; set; }
    public WorkoutLog WorkoutLog { get; set; } = null!;

    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int ActualSets { get; set; }
    public int? ActualReps { get; set; }
    public decimal? ActualWeightKg { get; set; }
    public int? ActualDurationSeconds { get; set; }
    public string? Notes { get; set; }
    public int OrderIndex { get; set; }
}

public class BodyMeasurement : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public decimal? WeightKg { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? MuscleMassKg { get; set; }
    public decimal? ChestCm { get; set; }
    public decimal? WaistCm { get; set; }
    public decimal? HipCm { get; set; }
    public decimal? ArmCm { get; set; }
    public decimal? LegCm { get; set; }
    public decimal? NeckCm { get; set; }
    public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

public class UserProgress : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime Date { get; set; }
    public int TotalWorkouts { get; set; }
    public int TotalCaloriesBurned { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalWorkoutMinutes { get; set; }
}
