using FastEndpoints;
using HCM.Domain.Identity.RefreshTokens;
using HCM.Features.Identity.Contracts;
using MediatR;

namespace HCM.Features.Identity.Login;

public sealed class Endpoint : Endpoint<LoginRequest,AuthResponse>
{
    private readonly IMediator mediator;
    private readonly TokenIssuer tokenIssuer;
    
    public Endpoint(IMediator mediator,TokenIssuer tokenIssuer)
    {
        this.mediator = mediator;
        this.tokenIssuer = tokenIssuer;
    }
    
    public override void Configure()
    {
        Post("api/auth/login");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(req.Email, req.Password), ct);
        
        if (result.IsFailure)
        {
            await SendResultAsync(
                Results.Problem(
                    detail: result.Error.Description,
                    statusCode: result.Error.Code
                )
            );
            return;
        }
        
        var tokenPair = await tokenIssuer.IssueNewTokensAsync(result.Value, null, ct);
        
        AuthCookieWriter.SetRefreshTokenCookie(HttpContext, tokenPair.RefreshToken);
        
        await SendOkAsync(tokenPair.ToAuthResponse(), cancellation: ct);
    }
}
