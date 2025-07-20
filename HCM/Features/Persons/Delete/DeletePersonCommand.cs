using MediatR;
using HCM.Shared.Results;

namespace HCM.Features.Persons.Delete;

public sealed record DeletePersonCommand(Guid PersonId) : IRequest<ResultWithoutValue>;
