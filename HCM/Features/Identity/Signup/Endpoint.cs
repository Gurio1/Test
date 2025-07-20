using FastEndpoints;
using HCM.Domain.Identity.RefreshTokens;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Identity.Signup;

public sealed class Endpoint : Endpoint<SignupRequest>
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
        Post("api/auth/signup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SignupRequest req, CancellationToken ct)
    {
        try
        {
            var result = await mediator.Send(new SignupCommand(req), ct);
            if (result.IsFailure)
            {
                await SendAsync(new { Error = result.Error.Description }, result.Error.Code, ct);
                return;
            }

            var tokenPair = result.Value;
            AuthCookieWriter.SetRefreshTokenCookie(HttpContext, tokenPair.RefreshToken);
            await SendOkAsync(tokenPair.ToAuthResponse(), cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during signup");
            ThrowError("An unexpected error occurred");
        }
    }
}
