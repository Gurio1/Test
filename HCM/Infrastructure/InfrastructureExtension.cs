using Microsoft.EntityFrameworkCore;

namespace HCM.Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager config)
    {
        services.AddDbContextPool<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(config.GetConnectionString("Postgres"));
        });
        
        return services;
    }
}
