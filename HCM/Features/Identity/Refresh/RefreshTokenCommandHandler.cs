using HCM.Domain.Persons;
using HCM.Shared.Results;
using HCM.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Identity.Refresh;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenPair>>
{
    private readonly TokenIssuer tokenIssuer;
    private readonly ApplicationDbContext context;
    private readonly ILogger<RefreshTokenCommandHandler> logger;

    public RefreshTokenCommandHandler(TokenIssuer tokenIssuer, ApplicationDbContext context, ILogger<RefreshTokenCommandHandler> logger)
    {
        this.tokenIssuer = tokenIssuer;
        this.context = context;
        this.logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var storedRefreshToken = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

            if (!RefreshTokenValidator.IsRefreshTokenValid(storedRefreshToken))
            {
                return Result<TokenPair>.Invalid("Invalid refresh token");
            }

            var user = await context.Persons.AsNoTracking().SingleOrDefaultAsync(u => u.Id == storedRefreshToken!.PersonId, cancellationToken);
            if (user is null)
            {
                return Result<TokenPair>.Invalid("Invalid refresh token");
            }

            var tokenPair = await tokenIssuer.IssueNewTokensAsync(user, storedRefreshToken, cancellationToken);
            return Result<TokenPair>.Success(tokenPair);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing token");
            return Result<TokenPair>.Failure("Failed to refresh token");
        }
    }
}
