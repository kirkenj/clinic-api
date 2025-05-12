using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;
public class ReceptionistRepository(ClinnicContext context) : IReceptionistRepository
{
    private readonly DbSet<Receptionist> _dbSet = context.Set<Receptionist>();

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(r => r.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(r => r.Email == email, cancellationToken);
    }

    public async Task<ICollection<Receptionist>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false)
    {
        return includeTracking 
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Receptionist?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(Receptionist receptionist, CancellationToken cancellationToken)
    {
        context.Update(receptionist);
        await context.SaveChangesAsync(cancellationToken);
    }
}
