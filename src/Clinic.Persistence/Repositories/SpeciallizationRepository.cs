using Clinic.Domain.Speciallization;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;
public class SpeciallizationRepository(ClinnicContext context) : ISpeciallizationRepository
{
    private readonly DbSet<Speciallization> _dbSet = context.Set<Speciallization>();

    public async Task AddAsync(Speciallization obj, CancellationToken cancellationToken)
    {
        _dbSet.Add(obj);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Speciallization obj, CancellationToken cancellationToken)
    {
        context.Remove(obj);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(o => o.Name == name, cancellationToken);
    }

    public async Task<ICollection<Speciallization>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false)
    {
        var set = !includeTracking ? _dbSet.AsNoTracking() : _dbSet;
        return await set.ToListAsync(cancellationToken);
    }

    public async Task<Speciallization?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(Speciallization newState, CancellationToken cancellationToken)
    {
        context.Update(newState);
        await context.SaveChangesAsync(cancellationToken);
    }
}