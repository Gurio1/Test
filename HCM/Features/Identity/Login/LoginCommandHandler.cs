using HCM.Domain.Persons;
using HCM.Infrastructure;
using HCM.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HCM.Features.Identity.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<Person>>
{
    private readonly ApplicationDbContext context;
    private readonly IPasswordHasher<Person> passwordHasher;
    
    public LoginCommandHandler(ApplicationDbContext context,IPasswordHasher<Person> passwordHasher)
    {
        this.context = context;
        this.passwordHasher = passwordHasher;
    }
    public async Task<Result<Person>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var person = await context.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
        
        if (person is null)
        {
            return Result<Person>.Invalid("Invalid email or password");
        }
        
        var passwordVerification = person.VerifyPassword(passwordHasher, request.Password);
        
        return passwordVerification == PasswordVerificationResult.Failed
            ? Result<Person>.Invalid("Invalid email or password")
            : Result<Person>.Success(person);
    }
}
