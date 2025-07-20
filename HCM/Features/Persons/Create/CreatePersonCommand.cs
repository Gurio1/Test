using HCM.Shared.Contracts;
using HCM.Shared.Results;
using MediatR;

namespace HCM.Features.Persons.Create;

public sealed record CreatePersonCommand(CreatePersonRequest Request) : IRequest<Result<PersonResponse>>;
