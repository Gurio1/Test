namespace HCM.Features.Persons.Create;

public record CreatePersonRequest(
    string FirstName,
    string LastName,
    string Email,
    string JobTitle,
    decimal Salary,
    string Password,
    string Role,
    string Department);
