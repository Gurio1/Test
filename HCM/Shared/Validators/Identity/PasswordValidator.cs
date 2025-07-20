using FluentValidation;

namespace HCM.Shared.Validators.Identity;

public static class PasswordValidator
{
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .MinimumLength(8)
            .WithMessage("Password cant be less than 8 symbols")
            .NotEmpty()
            .WithMessage("Password required")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*()_+\[\]{}:;<>,.?/~\\-]")
            .WithMessage("Password must contain at least one special symbol.");
}
