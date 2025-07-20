using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Identity.Logout;

public sealed class Endpoint : EndpointWithoutRequest
{
    private readonly IMediator mediator;
    private readonly ILogger<Endpoint> logger;

    public Endpoint(IMediator mediator, ILogger<Endpoint> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }
    
    public override void Configure() => Post("api/auth/logout");
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        string? cookieRefreshToken = HttpContext.Request.Cookies["refreshToken"];

        try
        {
            var result = await mediator.Send(new LogoutCommand(cookieRefreshToken), ct);
            if (result.IsFailure)
            {
                await SendResultAsync(Results.Problem(detail: result.Error.Description, statusCode: result.Error.Code));
                return;
            }

            await SendNoContentAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            ThrowError("An unexpected error occurred");
        }
    }
}
