using Clinic.Domain.Office;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class OfficeRepository : IOfficeRepository
{
    private readonly ClinnicContext _context;
    private readonly DbSet<Office> _dbSet;

    public OfficeRepository(ClinnicContext context)
    {
        _context = context;
        _dbSet = context.Set<Office>();
    }

    public async Task AddAsync(Office obj, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(obj, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(uint id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.FindAsync<Office>([id], cancellationToken: cancellationToken);
        _context.Remove(entity!);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExcistsByAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(o => o.Address == address, cancellationToken);
    }

    public async Task<ICollection<Office>> GetAllAsync(bool asTracking, CancellationToken cancellationToken = default)
    {
        var set = !asTracking ? _dbSet.AsNoTracking() : _dbSet;
        return await set.ToListAsync(cancellationToken);
    }

    public async Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(Office newState, CancellationToken cancellationToken = default)
    {
        _context.Update(newState);
        await _context.SaveChangesAsync(cancellationToken);
    }
}