using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class OfficeRepository(ClinnicContext context) : IOfficeRepository
{
    private readonly DbSet<Office> _dbSet = context.Set<Office>();

    public async Task AddAsync(Office office, CancellationToken cancellationToken)
    {
        await context.AddAsync(office, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Office office, CancellationToken cancellationToken)
    {
        context.Remove(office);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(o => o.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<ICollection<Office>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<ICollection<Office>> GetPaginatedAsync(uint page, uint pageSize, CancellationToken cancellationToken, bool includeTracking = false)
    {
        var set = includeTracking
            ? _dbSet.AsNoTracking()
            : _dbSet;

        return await set
            .OrderBy(o => o.Id)
            .Skip((int)((page - 1) * pageSize))
            .Take((int)pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Office office, CancellationToken cancellationToken)
    {
        context.Update(office);
        await context.SaveChangesAsync(cancellationToken);
    }
}