using FastEndpoints;
using HCM.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HCM.Features.Identity.Logout;

public sealed class Endpoint : EndpointWithoutRequest
{
    private readonly ApplicationDbContext context;
    
    public Endpoint(ApplicationDbContext context) => this.context = context;
    
    public override void Configure() => Post("api/auth/logout");
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        string? cookieRefreshToken = HttpContext.Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(cookieRefreshToken))
        {
            await SendNoContentAsync(ct);
            return;
        }
        
        await context.RefreshTokens
            .Where(t => t.Token == cookieRefreshToken)
            .ExecuteDeleteAsync(ct);
        
        await SendNoContentAsync(ct);
    }
}
