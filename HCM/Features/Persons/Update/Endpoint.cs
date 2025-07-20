using System.Security.Claims;
using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Domain.Persons;
using HCM.Infrastructure;
using HCM.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HCM.Features.Persons.Update;

public sealed class Endpoint :  Endpoint<UpdatePersonRequest,PersonResponse>
{
    private readonly ApplicationDbContext context;
    private readonly IAuthorizationService authorizationService;
    
    public Endpoint(ApplicationDbContext context, IAuthorizationService authorizationService)
    {
        this.context = context;
        this.authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Put(EndpointSettings.DefaultName + "/{PersonId}");
        Roles(ApplicationRoles.HrAdmin, ApplicationRoles.Manager);
    }
    
    public override async Task HandleAsync(UpdatePersonRequest req, CancellationToken ct)
    {
        var person = await context.Persons.FirstOrDefaultAsync(p => p.Id == req.PersonId, cancellationToken: ct);
        if (person == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var result = await authorizationService.AuthorizeAsync(User, person, "CanEditPerson");
        
        if (!result.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        person.Update(req, User.FindFirstValue(ClaimTypes.Role)!);
        
        await context.SaveChangesAsync(ct);
        
        await SendOkAsync(person.ToPersonResponse(), ct);
    }
}
