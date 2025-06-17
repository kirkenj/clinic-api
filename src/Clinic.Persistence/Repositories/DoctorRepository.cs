using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class DoctorRepository(ClinnicContext context) : IDoctorRepository
{
    private readonly DbSet<Doctor> _dbSet = context.Set<Doctor>();

    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        context.Add(doctor);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        context.Remove(doctor);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(d => EF.Functions.ILike(d.PhoneNumber, phoneNumber), cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(x => EF.Functions.ILike(x.Email, email), cancellationToken);
    }

    public Task<Doctor?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbSet.FirstOrDefaultAsync(x => EF.Functions.ILike(x.Email, email), cancellationToken);
    }

    public async Task<ICollection<Doctor>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Doctor?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Doctor>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);

        var collection = await _dbSet.OrderBy(o => o.Id)
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Doctor>(collection, page, pageSize, totalCount);
    }

    public Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        context.Update(doctor);
        return context.SaveChangesAsync(cancellationToken);
    }
}