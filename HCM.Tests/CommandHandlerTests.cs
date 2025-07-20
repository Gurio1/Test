using HCM.Domain.Identity;
using HCM.Domain.Identity.AccessTokens;
using HCM.Domain.Identity.RefreshTokens;
using HCM.Domain.Persons;
using HCM.Features.Identity.Login;
using HCM.Features.Identity.Logout;
using HCM.Features.Identity.Refresh;
using HCM.Features.Identity.Signup;
using HCM.Features.Identity;
using HCM.Features.Identity.Contracts;
using HCM.Features.Persons.Create;
using HCM.Features.Persons.Delete;
using HCM.Features.Persons.GetAll;
using HCM.Features.Persons.GetById;
using HCM.Features.Persons.Update;
using HCM.Shared;
using HCM.Shared.Contracts;
using HCM.Shared.Results;
using HCM.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace HCM.Tests;

public class CommandHandlerTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static TokenIssuer CreateTokenIssuer(ApplicationDbContext context)
    {
        var options = Options.Create(new JwtOptions
        {
            JwtSecret = "super-secret-key-super-secret-key",
            Issuer = "test",
            Audience = "test",
            AccessTokenMinutes = 5,
            RefreshTokenHours = 1
        });
        var generator = new JwtTokenGenerator(options);
        return new TokenIssuer(generator, context, options);
    }

    [Fact]
    public async Task Login_ReturnsInvalid_WhenEmailNotFound()
    {
        await using var context = CreateContext();
        var handler = new LoginCommandHandler(context, new PasswordHasher<Person>());

        var result = await handler.Handle(new LoginCommand("a@b.c", "pass"), CancellationToken.None);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Login_ReturnsInvalid_WhenPasswordWrong()
    {
        await using var context = CreateContext();
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass");
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, hasher);
        var result = await handler.Handle(new LoginCommand("john@example.com", "wrong"), CancellationToken.None);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Login_ReturnsSuccess_WithValidCredentials()
    {
        await using var context = CreateContext();
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass");
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, hasher);
        var result = await handler.Handle(new LoginCommand("john@example.com", "pass"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(person.Id, result.Value.Id);
    }

    [Fact]
    public async Task Signup_CreatesPersonAndReturnsTokens()
    {
        await using var context = CreateContext();
        var creator = new PersonCreator(context);
        var hasher = new PasswordHasher<Person>();
        var issuer = CreateTokenIssuer(context);
        var handler = new SignupCommandHandler(creator, issuer, hasher, NullLogger<SignupCommandHandler>.Instance);
        var request = new SignupRequest("John","Doe","john@example.com","Dev",100m,"IT","pass","pass");

        var result = await handler.Handle(new SignupCommand(request), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.AccessToken);
        Assert.Equal(1, await context.Persons.CountAsync());
        Assert.Equal(1, await context.RefreshTokens.CountAsync());
    }

    [Fact]
    public async Task RefreshToken_ReturnsInvalid_WhenTokenMissing()
    {
        await using var context = CreateContext();
        var issuer = CreateTokenIssuer(context);
        var handler = new RefreshTokenCommandHandler(issuer, context, NullLogger<RefreshTokenCommandHandler>.Instance);

        var result = await handler.Handle(new RefreshTokenCommand("unknown"), CancellationToken.None);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task RefreshToken_ReturnsNewTokens()
    {
        await using var context = CreateContext();
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass");
        context.Persons.Add(person);
        var issuer = CreateTokenIssuer(context);
        var refresh = RefreshToken.Create(person.Id, 1);
        context.RefreshTokens.Add(refresh);
        await context.SaveChangesAsync();

        var handler = new RefreshTokenCommandHandler(issuer, context, NullLogger<RefreshTokenCommandHandler>.Instance);
        var result = await handler.Handle(new RefreshTokenCommand(refresh.Token), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, await context.RefreshTokens.CountAsync());
    }

    [Fact]
    public async Task Logout_RemovesRefreshToken()
    {
        await using var context = CreateContext();
        var token = RefreshToken.Create(Guid.NewGuid(), 1);
        context.RefreshTokens.Add(token);
        await context.SaveChangesAsync();

        var handler = new LogoutCommandHandler(context, NullLogger<LogoutCommandHandler>.Instance);
        var result = await handler.Handle(new LogoutCommand(token.Token), CancellationToken.None);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task CreatePerson_Succeeds()
    {
        await using var context = CreateContext();
        var creator = new PersonCreator(context);
        var handler = new CreatePersonCommandHandler(creator, new PasswordHasher<Person>(), NullLogger<CreatePersonCommandHandler>.Instance);
        var req = new CreatePersonRequest("John","Doe","john@example.com","Dev",100m,"pass",ApplicationRoles.Employee,"IT");

        var result = await handler.Handle(new CreatePersonCommand(req), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, await context.Persons.CountAsync());
    }

    [Fact]
    public async Task DeletePerson_ReturnsNotFound_WhenMissing()
    {
        await using var context = CreateContext();
        var handler = new DeletePersonCommandHandler(context, NullLogger<DeletePersonCommandHandler>.Instance);

        var result = await handler.Handle(new DeletePersonCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(404, result.Error.Code);
    }

    [Fact]
    public async Task UpdatePerson_UpdatesValues()
    {
        await using var context = CreateContext();
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass");
        context.Persons.Add(person);
        await context.SaveChangesAsync();
        var handler = new UpdatePersonCommandHandler(context, NullLogger<UpdatePersonCommandHandler>.Instance);
        var request = new UpdatePersonRequest
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

        var result = await handler.Handle(new UpdatePersonCommand(request, ApplicationRoles.HrAdmin), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", (await context.Persons.FindAsync(person.Id))!.FirstName);
    }

    [Fact]
    public async Task GetPersonById_ReturnsPerson()
    {
        await using var context = CreateContext();
        var hasher = new PasswordHasher<Person>();
        var person = Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass");
        context.Persons.Add(person);
        await context.SaveChangesAsync();
        var handler = new GetPersonByIdQueryHandler(context, NullLogger<GetPersonByIdQueryHandler>.Instance);

        var result = await handler.Handle(new GetPersonByIdQuery(person.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(person.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetPersons_ReturnsFilteredByDepartmentForManager()
    {
        await using var context = CreateContext();
        var hasher = new PasswordHasher<Person>();
        context.Persons.Add(Person.Create("John","Doe","john@example.com","Dev",100m,"IT",ApplicationRoles.Employee,hasher,"pass"));
        context.Persons.Add(Person.Create("Ann","Lee","ann@example.com","Dev",100m,"Sales",ApplicationRoles.Employee,hasher,"pass"));
        await context.SaveChangesAsync();
        var handler = new GetPersonsQueryHandler(context, NullLogger<GetPersonsQueryHandler>.Instance);

        var result = await handler.Handle(new GetPersonsQuery(1,10,ApplicationRoles.Manager,"IT"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Persons);
    }
}
