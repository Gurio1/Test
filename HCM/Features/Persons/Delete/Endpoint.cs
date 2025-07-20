using FastEndpoints;
using HCM.Domain.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.Delete;

public sealed class Endpoint : Endpoint<DeletePersonRequest>
{
    private readonly IMediator mediator;
    private readonly ILogger<Endpoint> logger;

    public Endpoint(IMediator mediator, ILogger<Endpoint> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }
    
    public override void Configure()
    {
        Delete(EndpointSettings.DefaultName + "/{PersonId}");
        Roles(ApplicationRoles.HrAdmin);
    }
    
    public override async Task HandleAsync(DeletePersonRequest req, CancellationToken ct)
    {
        try
        {
            var result = await mediator.Send(new DeletePersonCommand(req.PersonId), ct);
            if (result.IsFailure)
            {
                await SendResultAsync(
                    Results.Problem(detail: result.Error.Description, statusCode: result.Error.Code));
                return;
            }

            await SendNoContentAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting person");
            ThrowError("An unexpected error occurred");
        }
    }
}
