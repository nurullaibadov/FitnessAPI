using FitnessAPI.Domain.Entities.Common;
using FitnessAPI.Domain.Enums;

namespace FitnessAPI.Domain.Entities;

public class WorkoutPlan : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public GoalType GoalType { get; set; }
    public int DurationWeeks { get; set; }
    public int DaysPerWeek { get; set; }
    public bool IsPublic { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }
    public int EstimatedCaloriesPerSession { get; set; }

    public ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();
    public ICollection<WorkoutLog> WorkoutLogs { get; set; } = new List<WorkoutLog>();
}

public class WorkoutDay : BaseEntity
{
    public Guid WorkoutPlanId { get; set; }
    public WorkoutPlan WorkoutPlan { get; set; } = null!;

    public int DayNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedDurationMinutes { get; set; }

    public ICollection<WorkoutDayExercise> WorkoutDayExercises { get; set; } = new List<WorkoutDayExercise>();
}

public class WorkoutDayExercise : BaseEntity
{
    public Guid WorkoutDayId { get; set; }
    public WorkoutDay WorkoutDay { get; set; } = null!;

    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int Sets { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public int RestSeconds { get; set; } = 60;
    public int OrderIndex { get; set; }
    public decimal? WeightKg { get; set; }
    public string? Notes { get; set; }
}
