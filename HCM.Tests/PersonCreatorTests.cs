using HCM.Domain.Identity;
using HCM.Domain.Persons;
using HCM.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using HCM.Infrastructure;

namespace HCM.Tests;

public class PersonCreatorTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Create_PersistsPerson()
    {
        using var context = CreateContext();
        var creator = new PersonCreator(context);
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass");

        var result = await creator.Create(person, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, await context.Persons.CountAsync());
    }

    private static ApplicationDbContext CreateFailingContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new ThrowingInterceptor())
            .Options;
        return new ApplicationDbContext(options);
    }

    private sealed class ThrowingInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
            => ValueTask.FromException<InterceptionResult<int>>(new DbUpdateException());
    }

    [Fact]
    public async Task Create_WhenDbUpdateFails_ReturnsFailure()
    {
        await using var context = CreateFailingContext();
        var creator = new PersonCreator(context);
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass");

        var result = await creator.Create(person, CancellationToken.None);

        Assert.True(result.IsFailure);
    }
}
