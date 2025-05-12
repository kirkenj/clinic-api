using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;
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
        return await _dbSet.AnyAsync(o => o.Name == name, cancellationToken);
    }

    public async Task<ICollection<Specialization>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false)
    {
        var set = includeTracking ? _dbSet : _dbSet.AsNoTracking();
        return await set.ToListAsync(cancellationToken);
    }

    public async Task<Specialization?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        context.Update(specialization);
        await context.SaveChangesAsync(cancellationToken);
    }
}