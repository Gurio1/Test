using FastEndpoints.Swagger;
using FluentValidation;

namespace HCM.Shared.Validators.Persons;

public static class NameValidator
{
    private const string NamePattern = @"^[\p{L} \.'\-]+$";
    
    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> rule, string propName) =>
        rule
            .NotEmpty().WithMessage($"{propName} is required.")
            .MaximumLength(50).WithMessage($"{propName} must be at most 50 characters.")
            .Matches(NamePattern).WithMessage($"{propName} contains invalid characters.")
            .SwaggerIgnore();
}
