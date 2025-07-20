using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Shared.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.Create;

public sealed class Endpoint : Endpoint<CreatePersonRequest, PersonResponse>
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
        Post(EndpointSettings.DefaultName);
        Roles(ApplicationRoles.HrAdmin);
        Description(d => d
            .WithSummary("Creates a new person")
            .WithDescription("Creates a new person record in the system")
            .Produces<PersonResponse>(201));
    }
    
    public override async Task HandleAsync(CreatePersonRequest req, CancellationToken ct)
    {
        try
        {
            var result = await mediator.Send(new CreatePersonCommand(req), ct);

            if (result.IsFailure)
            {
                await SendResultAsync(
                    Results.Problem(
                        detail: result.Error.Description,
                        statusCode: result.Error.Code
                    )
                );
                return;
            }

            await SendCreatedAtAsync<GetById.Endpoint>(
                new { id = result.Value.Id },
                result.Value, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating person");
            ThrowError("An unexpected error occurred");
        }
    }
}
