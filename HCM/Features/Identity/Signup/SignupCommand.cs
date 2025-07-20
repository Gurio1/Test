using MediatR;
using HCM.Shared.Results;

namespace HCM.Features.Identity.Signup;

public sealed record SignupCommand(SignupRequest Request) : IRequest<Result<TokenPair>>;
