using HCM.Domain.Persons;
using HCM.Infrastructure;
using HCM.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace HCM.Shared;

public sealed class PersonCreator
{
    private readonly ApplicationDbContext dbContext;
    
    public PersonCreator(ApplicationDbContext dbContext) => this.dbContext = dbContext;
    
    public async Task<Result<Person>> Create(Person person, CancellationToken cancellationToken)
    {
        dbContext.Persons.Add(person);
        
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result<Person>.Success(person);
        }
        catch (DbUpdateException ex)
        {
            return Result<Person>.Failure("An error occurred while creating the user");
        }
    }
}
