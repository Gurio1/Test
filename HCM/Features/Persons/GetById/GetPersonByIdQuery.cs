using HCM.Domain.Persons;
using HCM.Shared.Results;
using MediatR;

namespace HCM.Features.Persons.GetById;

public sealed record GetPersonByIdQuery(Guid PersonId) : IRequest<Result<Person>>;
