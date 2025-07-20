using HCM.Domain.Persons;
using HCM.Infrastructure;
using HCM.Shared.Contracts;
using HCM.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.Update;

public sealed class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Result<PersonResponse>>
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<UpdatePersonCommandHandler> logger;

    public UpdatePersonCommandHandler(ApplicationDbContext context, ILogger<UpdatePersonCommandHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<Result<PersonResponse>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var person = await context.Persons.FirstOrDefaultAsync(p => p.Id == request.Request.PersonId, cancellationToken);
            if (person == null)
                return Result<PersonResponse>.NotFound("Person not found");

            person.Update(request.Request, request.UserRole);
            await context.SaveChangesAsync(cancellationToken);
            return Result<PersonResponse>.Success(person.ToPersonResponse());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating person");
            return Result<PersonResponse>.Failure("Failed to update person");
        }
    }
}
