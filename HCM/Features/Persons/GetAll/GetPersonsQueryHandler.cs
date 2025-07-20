using HCM.Domain.Identity;
using HCM.Infrastructure;
using HCM.Shared.Contracts;
using HCM.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HCM.Features.Persons.GetAll;

public sealed class GetPersonsQueryHandler : IRequestHandler<GetPersonsQuery, Result<PagedResponse>>
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<GetPersonsQueryHandler> logger;

    public GetPersonsQueryHandler(ApplicationDbContext context, ILogger<GetPersonsQueryHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<Result<PagedResponse>> Handle(GetPersonsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var personsQueryable = context.Persons.AsNoTracking().AsQueryable();
            if (request.Role == ApplicationRoles.Manager)
            {
                personsQueryable = personsQueryable.Where(p => p.Department == request.Department);
            }

            int totalCount = await personsQueryable.CountAsync(cancellationToken);

            var persons = await personsQueryable
                .OrderBy(p => p.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => p.ToPersonResponse())
                .ToListAsync(cancellationToken);

            var response = new PagedResponse
            {
                Persons = persons,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            return Result<PagedResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting persons");
            return Result<PagedResponse>.Failure("Failed to retrieve persons");
        }
    }
}
