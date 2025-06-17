using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class ServiceRepository(ClinnicContext context) : IServiceRepository
{
    private readonly DbSet<Service> _dbSet = context.Set<Service>();

    public Task AddAsync(Service service, CancellationToken cancellationToken)
    {
        context.Add(service);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Service service, CancellationToken cancellationToken)
    {
        context.Remove(service);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(d => EF.Functions.ILike(d.Name, name), cancellationToken);
    }

    public async Task<ICollection<Service>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Service?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Service>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);

        var collection = await _dbSet.OrderBy(o => o.Id)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Service>(collection, page, pageSize, totalCount);
    }

    public Task UpdateAsync(Service service, CancellationToken cancellationToken)
    {
        context.Update(service);
        return context.SaveChangesAsync(cancellationToken);
    }
}