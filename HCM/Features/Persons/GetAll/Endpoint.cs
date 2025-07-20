using System.Security.Claims;
using FastEndpoints;
using HCM.Domain.Identity;
using HCM.Infrastructure;
using HCM.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HCM.Features.Persons.GetAll;

public sealed class Endpoint : Endpoint<GetPersonsRequest,PagedResponse>
{
    private readonly ApplicationDbContext context;
    
    public Endpoint(ApplicationDbContext context) => this.context = context;
    
    public override void Configure()
    {
        Get(EndpointSettings.DefaultName);
        Roles(ApplicationRoles.Manager,ApplicationRoles.HrAdmin);
    }
    
    public override async Task HandleAsync(GetPersonsRequest req, CancellationToken ct)
    {
        var queryablePersons = context.Persons.AsNoTracking().AsQueryable();
        string role = User.FindFirstValue(ClaimTypes.Role)!;
        
        if (role == ApplicationRoles.Manager)
        {
            queryablePersons = queryablePersons.Where(p => p.Department == User.FindFirstValue(ApplicationClaims.Department));
        }
        
        int totalCount = await queryablePersons.CountAsync(ct);
        
        var persons = await queryablePersons
            .OrderBy(p => p.Id)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(p => p.ToPersonResponse())
            .ToListAsync(ct);
        
        var response = new PagedResponse
        {
            Persons = persons,
            Page = req.Page,
            PageSize = req.PageSize,
            TotalCount = totalCount
        };
        
        await SendOkAsync(response, ct);
    }
}
