using Clinic.Domain;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence;

public class InnoClinnicDbContext(DbContextOptions<InnoClinnicDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<Receptionist> Receptionists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(InnoClinnicDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}