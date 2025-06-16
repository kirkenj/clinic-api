using Clinic.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Clinic.Persistence.Interceptors;

public sealed class AuditableEntitiesInterceptor(IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
        {
            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        var entries =
            dbContext
                .ChangeTracker
                .Entries<IAuditableEntity>();
        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = dateTimeProvider.UtcNow();
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.UpdatedOnUtc).CurrentValue = dateTimeProvider.UtcNow();
            }
        }

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }
}