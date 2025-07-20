using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Domain.Identity.RefreshTokens;
using HCM.Domain.Persons;
using HCM.Shared;
using Microsoft.AspNetCore.Identity;

namespace HCM.Features.Identity.Signup;

public sealed class Endpoint : Endpoint<SignupRequest>
{
    private readonly PersonCreator personCreator;
    private readonly TokenIssuer tokenIssuer;
    private readonly IPasswordHasher<Person> passwordHasher;
    
    public Endpoint(PersonCreator personCreator,TokenIssuer tokenIssuer,IPasswordHasher<Person> passwordHasher)
    {
        this.personCreator = personCreator;
        this.tokenIssuer = tokenIssuer;
        this.passwordHasher = passwordHasher;
    }
    
    public override void Configure()
    {
        Post("api/auth/signup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SignupRequest req, CancellationToken ct)
    {
        var person = Person.Create(req.FirstName, req.LastName, req.Email, req.JobTitle, req.Salary, req.Department,
            ApplicationRoles.Employee, passwordHasher, req.Password);
        
        var result = await personCreator.Create(person, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(new {Error = result.Error.Description}, result.Error.Code, ct);
            return;
        }
        
        var tokenPair = await tokenIssuer.IssueNewTokensAsync(result.Value, null, ct);
        
        AuthCookieWriter.SetRefreshTokenCookie(HttpContext, tokenPair.RefreshToken);
        
        await SendOkAsync(tokenPair.ToAuthResponse(), cancellation: ct);
    }
}
