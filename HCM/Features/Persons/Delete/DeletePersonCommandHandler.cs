using HCM.Infrastructure;
using HCM.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.Delete;

public sealed class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, ResultWithoutValue>
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<DeletePersonCommandHandler> logger;

    public DeletePersonCommandHandler(ApplicationDbContext context, ILogger<DeletePersonCommandHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<ResultWithoutValue> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var person = await context.Persons.FirstOrDefaultAsync(p => p.Id == request.PersonId, cancellationToken);
            if (person == null)
                return ResultWithoutValue.NotFound("Person not found");

            context.Persons.Remove(person);
            await context.SaveChangesAsync(cancellationToken);
            return ResultWithoutValue.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting person");
            return ResultWithoutValue.Failure("Failed to delete person");
        }
    }
}
