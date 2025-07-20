using FastEndpoints;
using HCM.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.GetById;

public sealed class Endpoint : Endpoint<GetPersonByIdRequest, PersonResponse>
{
    private readonly IMediator mediator;
    private readonly IAuthorizationService authorizationService;
    private readonly ILogger<Endpoint> logger;

    public Endpoint(IMediator mediator, IAuthorizationService authorizationService, ILogger<Endpoint> logger)
    {
        this.mediator = mediator;
        this.authorizationService = authorizationService;
        this.logger = logger;
    }
    
    public override void Configure() => Get(EndpointSettings.DefaultName + "/{PersonId}");
    
    public override async Task HandleAsync(GetPersonByIdRequest req, CancellationToken ct)
    {
        try
        {
            var result = await mediator.Send(new GetPersonByIdQuery(req.PersonId), ct);
            if (result.IsFailure)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var person = result.Value;
            var authResult = await authorizationService.AuthorizeAsync(User, person, "CanViewPerson");
            if (!authResult.Succeeded)
            {
                await SendForbiddenAsync(ct);
                return;
            }

            await SendOkAsync(person.ToPersonResponse(), ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving person by id");
            ThrowError("An unexpected error occurred");
        }
    }
}
