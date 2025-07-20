using System.Security.Claims;
using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Shared.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.GetAll;

public sealed class Endpoint : Endpoint<GetPersonsRequest, PagedResponse>
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
        Get(EndpointSettings.DefaultName);
        Roles(ApplicationRoles.Manager,ApplicationRoles.HrAdmin);
    }
    
    public override async Task HandleAsync(GetPersonsRequest req, CancellationToken ct)
    {
        try
        {
            string role = User.FindFirstValue(ClaimTypes.Role)!;
            string? department = User.FindFirstValue(ApplicationClaims.Department);

            var result = await mediator.Send(
                new GetPersonsQuery(req.Page, req.PageSize, role, department), ct);

            if (result.IsFailure)
            {
                await SendResultAsync(
                    Results.Problem(detail: result.Error.Description, statusCode: result.Error.Code));
                return;
            }

            await SendOkAsync(result.Value, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting persons");
            ThrowError("An unexpected error occurred");
        }
    }
}
