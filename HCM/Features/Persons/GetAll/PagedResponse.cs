using HCM.Shared.Contracts;

namespace HCM.Features.Persons.GetAll;

public sealed class PagedResponse
{
    public IEnumerable<PersonResponse> Persons { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
