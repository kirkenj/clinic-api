using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class DoctorRepository(ClinnicContext context) : IDoctorRepository
{
    private readonly DbSet<Doctor> _dbSet = context.Set<Doctor>();

    public async Task AddAsync(Doctor Doctor, CancellationToken cancellationToken)
    {
        await context.AddAsync(Doctor, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Doctor Doctor, CancellationToken cancellationToken)
    {
        context.Remove(Doctor);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.Email == email, cancellationToken);
    }

    public async Task<ICollection<Doctor>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Doctor?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(Doctor Doctor, CancellationToken cancellationToken)
    {
        context.Update(Doctor);
        await context.SaveChangesAsync(cancellationToken);
    }
}