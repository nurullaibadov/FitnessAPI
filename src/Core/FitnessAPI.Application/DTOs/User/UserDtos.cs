using FitnessAPI.Domain.Enums;

namespace FitnessAPI.Application.DTOs.User;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public GoalType GoalType { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class UserProfileDto : UserDto
{
    public int TotalWorkouts { get; set; }
    public int TotalCaloriesBurned { get; set; }
    public int CurrentStreak { get; set; }
    public int TotalWorkoutMinutes { get; set; }
    public DateTime? LastWorkoutDate { get; set; }
}

public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }
    public GoalType GoalType { get; set; }
    public string Role { get; set; } = "User";
}

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public GoalType? GoalType { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
}
