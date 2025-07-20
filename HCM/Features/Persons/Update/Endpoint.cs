using System.Security.Claims;
using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Shared.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HCM.Features.Persons.GetById;

namespace HCM.Features.Persons.Update;

public sealed class Endpoint : Endpoint<UpdatePersonRequest, PersonResponse>
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
    
    public override void Configure()
    {
        Put(EndpointSettings.DefaultName + "/{PersonId}");
        Roles(ApplicationRoles.HrAdmin, ApplicationRoles.Manager);
    }
    
    public override async Task HandleAsync(UpdatePersonRequest req, CancellationToken ct)
    {
        try
        {
            var getResult = await mediator.Send(new GetPersonByIdQuery(req.PersonId), ct);
            if (getResult.IsFailure)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var person = getResult.Value;
            var auth = await authorizationService.AuthorizeAsync(User, person, "CanEditPerson");
            if (!auth.Succeeded)
            {
                await SendForbiddenAsync(ct);
                return;
            }

            var updateResult = await mediator.Send(new UpdatePersonCommand(req, User.FindFirstValue(ClaimTypes.Role)!), ct);

            if (updateResult.IsFailure)
            {
                await SendResultAsync(Results.Problem(detail: updateResult.Error.Description, statusCode: updateResult.Error.Code));
                return;
            }

            await SendOkAsync(updateResult.Value, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating person");
            ThrowError("An unexpected error occurred");
        }
    }
}
