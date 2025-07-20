using HCM.Shared.Contracts;
using HCM.Shared.Results;
using MediatR;

namespace HCM.Features.Persons.Update;

public sealed record UpdatePersonCommand(UpdatePersonRequest Request, string UserRole) : IRequest<Result<PersonResponse>>;
