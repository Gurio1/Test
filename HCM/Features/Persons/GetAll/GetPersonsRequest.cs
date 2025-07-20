namespace HCM.Features.Persons.GetAll;

public sealed class GetPersonsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
