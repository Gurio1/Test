using FastEndpoints;
using FluentValidation;
using HCM.Features.Persons.Create;
using HCM.Infrastructure;
using HCM.Shared.Validators.Identity;
using HCM.Shared.Validators.Persons;

namespace HCM.Features.Identity.Signup;

public sealed class SignupRequestValidator: Validator<SignupRequest>
{
    public SignupRequestValidator(ApplicationDbContext context)
    {
        RuleFor(x => x.Email)
            .ValidEmail()
            .UniqueEmail(context);
        
        RuleFor(x => x.Password).ValidPassword();
        
        RuleFor(x => x.FirstName).ValidName(nameof(CreatePersonRequest.FirstName));
        
        RuleFor(x => x.LastName).ValidName(nameof(CreatePersonRequest.LastName));
        
        RuleFor(x => x.JobTitle).ValidJobTitle();
        
        RuleFor(x => x.Salary).ValidSalary();
        
        RuleFor(x => x.Password)
            .Equal(x => x.ConfirmedPassword)
            .WithMessage("Password and Confirmed password should be equal")
            .ValidPassword();
    }
}
