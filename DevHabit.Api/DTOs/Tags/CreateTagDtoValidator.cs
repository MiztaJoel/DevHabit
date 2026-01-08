using FluentValidation;

namespace DevHabit.Api.DTOs.Tags;

public sealed class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required.")
            .MinimumLength(3);
        RuleFor(x => x.Description)
            .MaximumLength(50).WithMessage("Description cannot exceed 50 characters.");
    }
}
