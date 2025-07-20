namespace HCM.Features.Persons.Update;

public sealed class UpdatePersonRequest
{
    public Guid PersonId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Department { get; set; }
    public required string Email { get; set; }
    public required string JobTitle { get; set; }
    public required decimal Salary { get; set; }
    public string? Role { get; set; }
}
