using HCM.Domain.Persons;

namespace HCM.Shared.Contracts;

public record PersonResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string JobTitle { get; set; }
    public decimal Salary { get; set; }
    public string Department { get; set; }
    public string Role { get; set; }
}

public static class Mapper
{
    public static PersonResponse ToPersonResponse(this Person person) =>
        new(){
            Id = person.Id.ToString(),
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            JobTitle = person.JobTitle,
            Salary = person.Salary,
            Department = person.Department,
            Role = person.Role
        };
}
