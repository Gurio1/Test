using FastEndpoints;
using HCM.Domain.Identity.RefreshTokens;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Identity.Refresh;

public sealed class Endpoint : EndpointWithoutRequest
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

        try
        {
            var result = await mediator.Send(new RefreshTokenCommand(cookieRefreshToken), ct);
            if (result.IsFailure)
            {
                await SendUnauthorizedAsync(ct);
                return;
            }

            var tokenPair = result.Value;
            AuthCookieWriter.SetRefreshTokenCookie(HttpContext, tokenPair.RefreshToken);
            await SendOkAsync(tokenPair.ToAuthResponse(), cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing token");
            ThrowError("An unexpected error occurred");
        }
    }
}
