using HCM.Domain.Identity.RefreshTokens;
using HCM.Domain.Persons;
using Microsoft.EntityFrameworkCore;

namespace HCM.Infrastructure;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
