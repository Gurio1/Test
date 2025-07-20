using FastEndpoints;
using HCM.Domain.Identity.RefreshTokens;
using HCM.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HCM.Features.Identity.Refresh;

public sealed class Endpoint : EndpointWithoutRequest
{
    private readonly TokenIssuer tokenIssuer;
    private readonly ApplicationDbContext context;
    private readonly ILogger<Endpoint> logger;
    
    public Endpoint(TokenIssuer tokenIssuer,ApplicationDbContext context, ILogger<Endpoint> logger)
    {
        this.tokenIssuer = tokenIssuer;
        this.context = context;
        this.logger = logger;
    }
    
    public override void Configure()
    {
        Post("api/auth/refresh-token");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        string? cookieRefreshToken = HttpContext.Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(cookieRefreshToken))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var storedRefreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == cookieRefreshToken, cancellationToken: ct);
        
        if (!RefreshTokenValidator.IsRefreshTokenValid(storedRefreshToken))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var user = await context.Persons
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == storedRefreshToken!.PersonId, cancellationToken: ct);
        
        if (user is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var tokenPair = await tokenIssuer.IssueNewTokensAsync(user, storedRefreshToken, ct);
        
        AuthCookieWriter.SetRefreshTokenCookie(HttpContext, tokenPair.RefreshToken);
        
        await SendOkAsync(tokenPair.ToAuthResponse(), cancellation: ct);
    }
}
