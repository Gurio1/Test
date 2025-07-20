using FluentValidation;

namespace HCM.Shared.Validators.Persons;

public static class JobTitleValidator
{
    public static IRuleBuilderOptions<T, string> ValidJobTitle<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Job title is required.")
            .MaximumLength(100).WithMessage("Job title must be at most 100 characters.");
}
