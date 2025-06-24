using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

    public async Task<PaginatedCollection<Doctor>> GetPaginatedAsync(DoctorFilter? doctorFilter, 
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var set = _dbSet.AsQueryable();
        if (doctorFilter is not null)
        {
            if (doctorFilter.Ids is not null && doctorFilter.Ids.Count != 0)
            {
                set = set.Where(o => doctorFilter.Ids.Contains(o.Id));
            }

            if (!string.IsNullOrEmpty(doctorFilter.Email))
            {
                set = set.Where(s => s.Email.Contains(doctorFilter.Email));
            }

            if (!string.IsNullOrEmpty(doctorFilter.MiddleName))
            {
                set = set.Where(s => s.MiddleName.Contains(doctorFilter.MiddleName));
            }

            if (!string.IsNullOrEmpty(doctorFilter.LastName))
            {
                set = set.Where(s => s.LastName.Contains(doctorFilter.LastName));
            }

            if (!string.IsNullOrEmpty(doctorFilter.FirstName))
            {
                set = set.Where(s => s.FirstName.Contains(doctorFilter.FirstName));
            }

            if (!string.IsNullOrEmpty(doctorFilter.PhoneNumber))
            {
                set = set.Where(s => s.PhoneNumber.Contains(doctorFilter.PhoneNumber));
            }

            if (doctorFilter.IsActive.HasValue)
            {
                set = set.Where(s => s.IsActive == doctorFilter.IsActive.Value);
            }

            if (doctorFilter.OfficeIds is not null && doctorFilter.OfficeIds.Count != 0)
            {
                set = set.Where(o => doctorFilter.OfficeIds.Contains(o.OfficeId));
            }

            if (doctorFilter.SpecializationIds is not null && doctorFilter.SpecializationIds.Count != 0)
            {
                set = set.Where(o => doctorFilter.SpecializationIds.Contains(o.SpecializationId));
            }

            set = doctorFilter.OrderDescending
                ? set.OrderByDescending(GetSortProperty(doctorFilter))
                : set.OrderBy(GetSortProperty(doctorFilter));
        }
        else
        {
            set = set.OrderBy(s => s.Id);
        }

        var totalCount = await set.CountAsync(cancellationToken);

        var collection = await set.AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Doctor>(collection, page, pageSize, totalCount);
    }

    private Expression<Func<Doctor, object>> GetSortProperty(DoctorFilter doctorFilter)
    {
        return doctorFilter.SortColumn switch
        {
            DoctorSortColumn.Id => patient => patient.Id,
            DoctorSortColumn.FirstName => patient => patient.FirstName,
            DoctorSortColumn.LastName => patient => patient.LastName,
            DoctorSortColumn.MiddleName => patient => patient.MiddleName,
            DoctorSortColumn.Email => patient => patient.Email,
            DoctorSortColumn.PhoneNumber => patient => patient.PhoneNumber,
            DoctorSortColumn.IsActive => patient => patient.IsActive,
            DoctorSortColumn.SpecializationId => patient => patient.SpecializationId,
            DoctorSortColumn.OfficeId => patient => patient.OfficeId,
            _ => patient => patient.Id
        };
    }

    public Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        context.Update(doctor);
        return context.SaveChangesAsync(cancellationToken);
    }
}