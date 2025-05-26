using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.DapperPersistence.Repositories;

public sealed class PatientRepository(ClinnicContext context) : IPatientRepository
{
    private readonly DbSet<Patient> _dbSet = context.Set<Patient>();

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        await context.AddAsync(patient, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Patient patient, CancellationToken cancellationToken)
    {
        context.Remove(patient);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<ICollection<Patient>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Patient?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Patient>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);

        var collection = await _dbSet.OrderBy(o => o.Id)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Patient>(collection, page, pageSize, totalCount);
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken)
    {
        context.Update(patient);
        await context.SaveChangesAsync(cancellationToken);
    }
}