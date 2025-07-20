using HCM.Domain.Identity;
using HCM.Features.Persons.Update;
using Microsoft.AspNetCore.Identity;

namespace HCM.Domain.Persons;

public sealed class Person
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string JobTitle { get; set; }
    public decimal Salary { get; set; }
    public string Department { get; set; }
    public string Role { get; set; }
    public string PasswordHash { get; private set; }
    
    private Person(){}
    
    public static Person Create(
        string firstName,
        string lastName,
        string email,
        string jobTitle,
        decimal salary,
        string department,
        string role,
        IPasswordHasher<Person> passwordHasher,
        string password)
    {
        var user = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            JobTitle = jobTitle,
            Salary = salary,
            Department = department,
            Role = role
        };
        user.PasswordHash = passwordHasher.HashPassword(user, password);
        return user;
    }
    
    public Person Update(UpdatePersonRequest request, string role)
    {
        FirstName = request.FirstName;
        LastName = request.LastName;
        Email = request.Email;
        JobTitle = request.JobTitle;
        Salary = request.Salary;
        Department = request.Department;
        
        if (role != ApplicationRoles.HrAdmin) return this;
        
        if (request.Role is not null)
        {
            Role = request.Role;
        }
        
        return this;
    }
    
    public PasswordVerificationResult VerifyPassword(
        IPasswordHasher<Person> passwordHasher, string password) =>
        passwordHasher.VerifyHashedPassword(this, PasswordHash, password);
}
