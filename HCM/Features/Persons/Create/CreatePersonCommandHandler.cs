using HCM.Domain.Persons;
using HCM.Shared;
using HCM.Shared.Contracts;
using HCM.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.Create;

public sealed class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Result<PersonResponse>>
{
    private readonly PersonCreator personCreator;
    private readonly IPasswordHasher<Person> passwordHasher;
    private readonly ILogger<CreatePersonCommandHandler> logger;

    public CreatePersonCommandHandler(PersonCreator personCreator, IPasswordHasher<Person> passwordHasher, ILogger<CreatePersonCommandHandler> logger)
    {
        this.personCreator = personCreator;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }

    public async Task<Result<PersonResponse>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var r = request.Request;
            var person = Person.Create(r.FirstName, r.LastName, r.Email, r.JobTitle, r.Salary, r.Department,
                r.Role, passwordHasher, r.Password);

            var result = await personCreator.Create(person, cancellationToken);

            if (result.IsFailure)
            {
                logger.LogWarning("Failed to create person: {Error}", result.Error.Description);
                return result.AsError<PersonResponse>();
            }

            return Result<PersonResponse>.Success(person.ToPersonResponse());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception during person creation");
            return Result<PersonResponse>.Failure("An error occurred while creating the person");
        }
    }
}
