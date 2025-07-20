using System.Security.Claims;
using HCM.Domain.Identity.Authentication.Requirements;
using HCM.Domain.Persons;
using Microsoft.AspNetCore.Authorization;

namespace HCM.Domain.Identity.Authentication.RequirementHandlers;

public sealed class ViewPersonRequirementHandler  : AuthorizationHandler<ViewPersonRequirement, Person>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ViewPersonRequirement requirement, Person resource)
    {
        string? personId = context.User.FindFirstValue(ApplicationClaims.PersonId);
        string? role = context.User.FindFirstValue(ClaimTypes.Role);
        string? department = context.User.FindFirstValue(ApplicationClaims.Department);
        
        if (role == ApplicationRoles.HrAdmin ||
            (role == ApplicationRoles.Manager && resource.Department == department) ||
            (role == ApplicationRoles.Employee && resource.Id.ToString() == personId))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
