using HCM.Domain.Identity;
using HCM.Domain.Persons;
using HCM.Features.Identity.Contracts;
using HCM.Shared;
using HCM.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Identity.Signup;

public sealed class SignupCommandHandler : IRequestHandler<SignupCommand, Result<TokenPair>>
{
    private readonly PersonCreator personCreator;
    private readonly TokenIssuer tokenIssuer;
    private readonly IPasswordHasher<Person> passwordHasher;
    private readonly ILogger<SignupCommandHandler> logger;

    public SignupCommandHandler(PersonCreator personCreator, TokenIssuer tokenIssuer, IPasswordHasher<Person> passwordHasher, ILogger<SignupCommandHandler> logger)
    {
        this.personCreator = personCreator;
        this.tokenIssuer = tokenIssuer;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var r = request.Request;
            var person = Person.Create(r.FirstName, r.LastName, r.Email, r.JobTitle, r.Salary, r.Department,
                ApplicationRoles.Employee, passwordHasher, r.Password);

            var creationResult = await personCreator.Create(person, cancellationToken);

            if (creationResult.IsFailure)
            {
                logger.LogWarning("Signup failed: {Error}", creationResult.Error.Description);
                return creationResult.AsError<TokenPair>();
            }

            var tokenPair = await tokenIssuer.IssueNewTokensAsync(creationResult.Value, null, cancellationToken);
            return Result<TokenPair>.Success(tokenPair);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during signup");
            return Result<TokenPair>.Failure("Failed to signup");
        }
    }
}
