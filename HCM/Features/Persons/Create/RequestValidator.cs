using FastEndpoints;
using FluentValidation;
using HCM.Infrastructure;
using HCM.Shared.Validators.Identity;
using HCM.Shared.Validators.Persons;

namespace HCM.Features.Persons.Create;

public sealed class RequestValidator : Validator<CreatePersonRequest>
{
    
    public RequestValidator(ApplicationDbContext context)
    {
        RuleFor(x => x.Email)
            .ValidEmail()
            .UniqueEmail(context);
        
        RuleFor(x => x.Password).ValidPassword();
        
        RuleFor(x => x.FirstName).ValidName(nameof(CreatePersonRequest.FirstName));
        
        RuleFor(x => x.LastName).ValidName(nameof(CreatePersonRequest.LastName));
        
        RuleFor(x => x.JobTitle).ValidJobTitle();
        
        RuleFor(x => x.Salary).ValidSalary();
        
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role can not be empty");
    }
}
