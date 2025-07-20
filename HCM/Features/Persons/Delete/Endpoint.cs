using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HCM.Features.Persons.Delete;

public sealed class Endpoint : Endpoint<DeletePersonRequest>
{
    private readonly ApplicationDbContext context;
    
    public Endpoint(ApplicationDbContext context) => this.context = context;
    
    public override void Configure()
    {
        Delete(EndpointSettings.DefaultName + "/{PersonId}");
        Roles(ApplicationRoles.HrAdmin);
    }
    
    public override async Task HandleAsync(DeletePersonRequest req, CancellationToken ct)
    {
        var person = await context.Persons
            .FirstOrDefaultAsync(p => p.Id == req.PersonId, ct);
        if (person == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        context.Persons.Remove(person);
        await context.SaveChangesAsync(ct);
        
        await SendNoContentAsync(ct);
    }
}
