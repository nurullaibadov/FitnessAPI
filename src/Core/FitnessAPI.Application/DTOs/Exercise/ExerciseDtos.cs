using FitnessAPI.Domain.Enums;

namespace FitnessAPI.Application.DTOs.Exercise;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid? MuscleGroupId { get; set; }
    public string? MuscleGroupName { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public ExerciseType ExerciseType { get; set; }
    public string? VideoUrl { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? CaloriePerMinute { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? MuscleGroupId { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public ExerciseType ExerciseType { get; set; }
    public string? VideoUrl { get; set; }
    public decimal? CaloriePerMinute { get; set; }
}

public class UpdateExerciseDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? MuscleGroupId { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
    public ExerciseType? ExerciseType { get; set; }
    public string? VideoUrl { get; set; }
    public decimal? CaloriePerMinute { get; set; }
    public bool? IsApproved { get; set; }
}
