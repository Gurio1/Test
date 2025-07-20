using FastEndpoints;
using HCM.Infrastructure;
using HCM.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HCM.Features.Persons.GetById;

public sealed class Endpoint : Endpoint<GetPersonByIdRequest, PersonResponse>
{
    private readonly ApplicationDbContext context;
    private readonly IAuthorizationService authorizationService;
    
    public Endpoint(ApplicationDbContext context, IAuthorizationService authorizationService)
    {
        this.context = context;
        this.authorizationService = authorizationService;
    }
    
    public override void Configure() => Get(EndpointSettings.DefaultName + "/{PersonId}");
    
    public override async Task HandleAsync(GetPersonByIdRequest req, CancellationToken ct)
    {
        var person = await context.Persons.AsNoTracking().FirstOrDefaultAsync(p => p.Id == req.PersonId, cancellationToken: ct);
        if (person == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var result = await authorizationService.AuthorizeAsync(User, person, "CanViewPerson");
        
        if (!result.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        await SendOkAsync(person.ToPersonResponse(), ct);
    }
}
