namespace FitnessAPI.Application.DTOs.WorkoutLog;

public class WorkoutLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? WorkoutPlanId { get; set; }
    public string? WorkoutPlanName { get; set; }
    public Guid? WorkoutDayId { get; set; }
    public string? WorkoutDayName { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalDurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public int OverallRating { get; set; }
    public ICollection<WorkoutLogDetailDto> Details { get; set; } = new List<WorkoutLogDetailDto>();
}

public class WorkoutLogDetailDto
{
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int ActualSets { get; set; }
    public int? ActualReps { get; set; }
    public decimal? ActualWeightKg { get; set; }
    public int? ActualDurationSeconds { get; set; }
    public string? Notes { get; set; }
}

public class CreateWorkoutLogDto
{
    public Guid? WorkoutPlanId { get; set; }
    public Guid? WorkoutDayId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int TotalDurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public int OverallRating { get; set; } = 5;
    public ICollection<CreateWorkoutLogDetailDto> Details { get; set; } = new List<CreateWorkoutLogDetailDto>();
}

public class CreateWorkoutLogDetailDto
{
    public Guid ExerciseId { get; set; }
    public int ActualSets { get; set; }
    public int? ActualReps { get; set; }
    public decimal? ActualWeightKg { get; set; }
    public int? ActualDurationSeconds { get; set; }
    public string? Notes { get; set; }
    public int OrderIndex { get; set; }
}
