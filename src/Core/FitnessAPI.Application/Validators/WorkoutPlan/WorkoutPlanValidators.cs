using FluentValidation;
using FitnessAPI.Application.DTOs.WorkoutPlan;

namespace FitnessAPI.Application.Validators.WorkoutPlan;

public class CreateWorkoutPlanValidator : AbstractValidator<CreateWorkoutPlanDto>
{
    public CreateWorkoutPlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workout plan name is required.")
            .MaximumLength(100);

        RuleFor(x => x.DurationWeeks)
            .GreaterThan(0).WithMessage("Duration must be at least 1 week.")
            .LessThanOrEqualTo(52).WithMessage("Duration cannot exceed 52 weeks.");

        RuleFor(x => x.DaysPerWeek)
            .InclusiveBetween(1, 7).WithMessage("Days per week must be between 1 and 7.");

        RuleFor(x => x.WorkoutDays)
            .NotEmpty().WithMessage("At least one workout day is required.");
    }
}
