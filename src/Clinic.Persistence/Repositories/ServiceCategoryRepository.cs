using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.DapperPersistence.Repositories;

public sealed class ServiceCategoryRepository(ClinnicContext context) : IServiceCategoryRepository
{
    private readonly DbSet<ServiceCategory> _dbSet = context.Set<ServiceCategory>();

    public async Task AddAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken)
    {
        await context.AddAsync(serviceCategory, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken)
    {
        context.Remove(serviceCategory);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(o => o.Name == name, cancellationToken);
    }

    public async Task<ICollection<ServiceCategory>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<ServiceCategory?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<ServiceCategory>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);

        var collection = await _dbSet.OrderBy(o => o.Id)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<ServiceCategory>(collection, page, pageSize, totalCount);
    }

    public async Task UpdateAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken)
    {
        context.Update(serviceCategory);
        await context.SaveChangesAsync(cancellationToken);
    }
}