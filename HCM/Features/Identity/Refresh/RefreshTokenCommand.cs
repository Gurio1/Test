using MediatR;
using HCM.Shared.Results;

namespace HCM.Features.Identity.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenPair>>;
