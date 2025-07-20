namespace HCM.Features.Persons.GetById;

public record GetPersonByIdRequest
{
    public required Guid PersonId { get; set; }
}
