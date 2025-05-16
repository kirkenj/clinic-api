using Clinic.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Clinic.Persistence.Interceptors;

public sealed class AuditableEntitiesInterceptor(ILogger<AuditableEntitiesInterceptor> logger) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Interceptor Executed");

        var dbContext = eventData.Context;

        if (dbContext is null)
            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);

        var entries =
            dbContext
                .ChangeTracker
                .Entries<IAuditableEntity>();

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
                entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Modified)
                entityEntry.Property(a => a.UpdatedOnUtc).CurrentValue = DateTime.UtcNow;
        }

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }
}