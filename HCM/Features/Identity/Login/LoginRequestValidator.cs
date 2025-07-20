using FastEndpoints;
using HCM.Shared.Validators.Persons;

namespace HCM.Features.Identity.Login;

public sealed class RequestValidator : Validator<LoginRequest>
{
    public RequestValidator() =>
        RuleFor(x => x.Email).ValidEmail();
}
