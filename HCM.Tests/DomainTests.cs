using HCM.Domain.Identity;
using HCM.Domain.Persons;
using HCM.Features.Persons.Update;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace HCM.Tests;

public class DomainTests
{
    [Fact]
    public void Create_HashesPassword()
    {
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John", "Doe", "john@example.com", "Dev", 100m, "IT", ApplicationRoles.Employee, hasher, "pass");

        Assert.NotEqual("pass", person.PasswordHash);
        Assert.Equal(PasswordVerificationResult.Success, person.VerifyPassword(hasher, "pass"));
    }

    [Fact]
    public void Update_ByNonHr_DoesNotChangeRole()
    {
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John", "Doe", "john@example.com", "Dev", 100m, "IT", ApplicationRoles.Employee, hasher, "pass");
        var update = new UpdatePersonRequest
        {
            PersonId = person.Id,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "john@example.com",
            JobTitle = "Dev",
            Salary = 200m,
            Department = "IT",
            Role = ApplicationRoles.Manager
        };

        person.Update(update, ApplicationRoles.Manager);

        Assert.Equal(ApplicationRoles.Employee, person.Role);
        Assert.Equal("Jane", person.FirstName);
    }

    [Fact]
    public void Update_ByHr_ChangesRole()
    {
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John", "Doe", "john@example.com", "Dev", 100m, "IT", ApplicationRoles.Employee, hasher, "pass");
        var update = new UpdatePersonRequest
        {
            PersonId = person.Id,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "john@example.com",
            JobTitle = "Dev",
            Salary = 200m,
            Department = "IT",
            Role = ApplicationRoles.Manager
        };

        person.Update(update, ApplicationRoles.HrAdmin);

        Assert.Equal(ApplicationRoles.Manager, person.Role);
    }
}
