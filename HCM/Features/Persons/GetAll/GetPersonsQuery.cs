using HCM.Shared.Results;
using MediatR;

namespace HCM.Features.Persons.GetAll;

public sealed record GetPersonsQuery(int Page, int PageSize, string Role, string? Department) : IRequest<Result<PagedResponse>>;
