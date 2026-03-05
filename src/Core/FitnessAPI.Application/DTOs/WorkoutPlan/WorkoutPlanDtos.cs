using FitnessAPI.Domain.Enums;

namespace FitnessAPI.Application.DTOs.WorkoutPlan;

public class WorkoutPlanDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public GoalType GoalType { get; set; }
    public int DurationWeeks { get; set; }
    public int DaysPerWeek { get; set; }
    public bool IsPublic { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public int EstimatedCaloriesPerSession { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<WorkoutDayDto> WorkoutDays { get; set; } = new List<WorkoutDayDto>();
}

public class WorkoutDayDto
{
    public Guid Id { get; set; }
    public int DayNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public ICollection<WorkoutDayExerciseDto> Exercises { get; set; } = new List<WorkoutDayExerciseDto>();
}

public class WorkoutDayExerciseDto
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string? ExerciseImageUrl { get; set; }
    public int Sets { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public int RestSeconds { get; set; }
    public int OrderIndex { get; set; }
    public string? Notes { get; set; }
}

public class CreateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public GoalType GoalType { get; set; }
    public int DurationWeeks { get; set; }
    public int DaysPerWeek { get; set; }
    public bool IsPublic { get; set; } = false;
    public ICollection<CreateWorkoutDayDto> WorkoutDays { get; set; } = new List<CreateWorkoutDayDto>();
}

public class CreateWorkoutDayDto
{
    public int DayNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<CreateWorkoutDayExerciseDto> Exercises { get; set; } = new List<CreateWorkoutDayExerciseDto>();
}

public class CreateWorkoutDayExerciseDto
{
    public Guid ExerciseId { get; set; }
    public int Sets { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public int RestSeconds { get; set; } = 60;
    public int OrderIndex { get; set; }
    public string? Notes { get; set; }
}

public class UpdateWorkoutPlanDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
    public GoalType? GoalType { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsActive { get; set; }
}
