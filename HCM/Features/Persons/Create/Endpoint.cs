using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Domain.Persons;
using HCM.Shared;
using HCM.Shared.Contracts;
using Microsoft.AspNetCore.Identity;

namespace HCM.Features.Persons.Create;

public sealed class Endpoint : Endpoint<CreatePersonRequest,PersonResponse>
{
    private readonly PersonCreator personCreator;
    private readonly IPasswordHasher<Person> passwordHasher;
    
    public Endpoint(PersonCreator personCreator,IPasswordHasher<Person> passwordHasher)
    {
        this.personCreator = personCreator;
        this.passwordHasher = passwordHasher;
    }
    
    public override void Configure()
    {
        Post(EndpointSettings.DefaultName);
        Roles(ApplicationRoles.HrAdmin);
        Description(d => d
            .WithSummary("Creates a new person")
            .WithDescription("Creates a new person record in the system")
            .Produces<PersonResponse>(201));
    }
    
    public override async Task HandleAsync(CreatePersonRequest req, CancellationToken ct)
    {
        var person = Person.Create(req.FirstName, req.LastName, req.Email, req.JobTitle, req.Salary, req.Department,
            req.Role, passwordHasher, req.Password);
        
        var personCreationResult = await personCreator.Create(person, ct);
        
        if (personCreationResult.IsFailure)
        {
            await SendResultAsync(
                Results.Problem(
                    detail:    personCreationResult.Error.Description,
                    statusCode: personCreationResult.Error.Code
                )
            );
            return;
        }
        
        await SendCreatedAtAsync<GetById.Endpoint>(
            new { id = person.Id },
            person.ToPersonResponse(), cancellation: ct);
    }
}
