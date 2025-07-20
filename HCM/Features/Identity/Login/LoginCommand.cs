using HCM.Domain.Persons;
using HCM.Shared.Results;
using MediatR;

namespace HCM.Features.Identity.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<Person>>;
