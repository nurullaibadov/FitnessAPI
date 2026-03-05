using FitnessAPI.Domain.Entities.Common;
using FitnessAPI.Domain.Enums;

namespace FitnessAPI.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}

public class MuscleGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? BodyPart { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}

public class Exercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid? MuscleGroupId { get; set; }
    public MuscleGroup? MuscleGroup { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public ExerciseType ExerciseType { get; set; }
    public string? VideoUrl { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? CaloriePerMinute { get; set; }
    public bool IsApproved { get; set; } = false;
    public Guid? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public ICollection<WorkoutDayExercise> WorkoutDayExercises { get; set; } = new List<WorkoutDayExercise>();
}
