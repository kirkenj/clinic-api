using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence;

public class ClinnicContext(DbContextOptions<ClinnicContext> dbContextOptions) : DbContext(dbContextOptions)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ClinnicContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}