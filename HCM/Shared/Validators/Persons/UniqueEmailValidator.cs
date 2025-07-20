using FluentValidation;
using HCM.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HCM.Shared.Validators.Persons;

public static class UniqueEmailValidator
{
    public static IRuleBuilderOptions<T, string> UniqueEmail<T>(this IRuleBuilder<T, string> rule,ApplicationDbContext context) =>
        rule
            .MustAsync(async (email, ct) => !await context.Persons.AnyAsync(u => u.Email == email, ct))
            .WithMessage("This email is already taken.");
}
