using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.DapperPersistence.Repositories;

public class SpecializationRepository(ClinnicContext context) : ISpecializationRepository
{
    private readonly DbSet<Specialization> _dbSet = context.Set<Specialization>();

    public async Task AddAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        _dbSet.Add(specialization);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        context.Remove(specialization);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<ICollection<Specialization>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Specialization?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Specialization>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);

        var collection = await _dbSet.OrderBy(o => o.Id)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Specialization>(collection, page, pageSize, totalCount);
    }

    public async Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        context.Update(specialization);
        await context.SaveChangesAsync(cancellationToken);
    }
}