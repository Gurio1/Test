using FastEndpoints.Swagger;
using FluentValidation;

namespace HCM.Shared.Validators.Persons;

public static class SalaryValidator
{
    public static IRuleBuilderOptions<T, decimal> ValidSalary<T>(
        this IRuleBuilder<T, decimal> ruleBuilder) =>
        ruleBuilder
            .GreaterThanOrEqualTo(0).WithMessage("Salary must be a non-negative number.")
            .LessThanOrEqualTo(1_000_000).WithMessage("Salary is unrealistically high.");
}
