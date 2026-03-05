using FluentValidation;
using FitnessAPI.Application.DTOs.Exercise;

namespace FitnessAPI.Application.Validators.Exercise;

public class CreateExerciseValidator : AbstractValidator<CreateExerciseDto>
{
    public CreateExerciseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exercise name is required.")
            .MaximumLength(100);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
    }
}
