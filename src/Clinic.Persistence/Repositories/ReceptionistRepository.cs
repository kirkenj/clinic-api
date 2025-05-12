using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;
public class ReceptionistRepository(ClinnicContext context) : IReceptionistRepository
{
    private readonly DbSet<Receptionist> _dbSet = context.Set<Receptionist>();

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(o => o.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(o => o.PhoneNumber == email, cancellationToken);
    }

    public async Task<ICollection<Receptionist>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false)
    {
        var set = includeTracking ? _dbSet : _dbSet.AsNoTracking();
        return await set.ToListAsync(cancellationToken);
    }

    public async Task<Receptionist?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(Receptionist Receptionist, CancellationToken cancellationToken)
    {
        context.Update(Receptionist);
        await context.SaveChangesAsync(cancellationToken);
    }
}
