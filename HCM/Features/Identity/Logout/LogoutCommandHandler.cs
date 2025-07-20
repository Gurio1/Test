using HCM.Infrastructure;
using HCM.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Identity.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResultWithoutValue>
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<LogoutCommandHandler> logger;

    public LogoutCommandHandler(ApplicationDbContext context, ILogger<LogoutCommandHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<ResultWithoutValue> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return ResultWithoutValue.Success();
        }

        try
        {
            await context.RefreshTokens.Where(t => t.Token == request.RefreshToken).ExecuteDeleteAsync(cancellationToken);
            return ResultWithoutValue.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging out");
            return ResultWithoutValue.Failure("Failed to logout");
        }
    }
}
