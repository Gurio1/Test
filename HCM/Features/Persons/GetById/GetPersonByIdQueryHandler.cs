using HCM.Domain.Persons;
using HCM.Infrastructure;
using HCM.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.GetById;

public sealed class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Result<Person>>
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<GetPersonByIdQueryHandler> logger;

    public GetPersonByIdQueryHandler(ApplicationDbContext context, ILogger<GetPersonByIdQueryHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<Result<Person>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var person = await context.Persons.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.PersonId, cancellationToken);
            return person is null
                ? Result<Person>.NotFound("Person not found")
                : Result<Person>.Success(person);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting person by id");
            return Result<Person>.Failure("Failed to retrieve person");
        }
    }
}
