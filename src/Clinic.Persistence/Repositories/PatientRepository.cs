using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class PatientRepository(ClinnicContext context) : IPatientRepository
{
    private readonly DbSet<Patient> _dbSet = context.Set<Patient>();

    public Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        context.Add(patient);
        return context.SaveChangesAsync(cancellationToken);
    } 

    public Task DeleteAsync(Patient patient, CancellationToken cancellationToken)
    {
        context.Remove(patient);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(p => EF.Functions.ILike(p.PhoneNumber, phoneNumber), cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(p => EF.Functions.ILike(p.Email, email), cancellationToken);
    }

    public Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbSet.FirstOrDefaultAsync(x => EF.Functions.ILike(x.Email, email), cancellationToken);
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
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Patient>(collection, page, pageSize, totalCount);
    }

    public Task UpdateAsync(Patient patient, CancellationToken cancellationToken)
    {
        context.Update(patient);
        return context.SaveChangesAsync(cancellationToken);
    }
}