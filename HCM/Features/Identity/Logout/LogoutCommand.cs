using MediatR;
using HCM.Shared.Results;

namespace HCM.Features.Identity.Logout;

public sealed record LogoutCommand(string? RefreshToken) : IRequest<ResultWithoutValue>;
