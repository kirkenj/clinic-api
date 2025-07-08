using System.Linq.Expressions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
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

    public async Task<Patient?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Patient>> GetPaginatedAsync(PatientFilter? patientFilter, 
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var set = _dbSet.AsQueryable().Where(d => d.IsActive);
        if (patientFilter is not null)
        {
            if (patientFilter.Ids is not null && patientFilter.Ids.Count != 0)
            {
                set = set.Where(o => patientFilter.Ids.Contains(o.Id));
            }

            if (!string.IsNullOrEmpty(patientFilter.Email))
            {
                set = set.Where(s => s.Email.Contains(patientFilter.Email));
            }

            if (!string.IsNullOrEmpty(patientFilter.MiddleName))
            {
                set = set.Where(s => s.MiddleName.Contains(patientFilter.MiddleName));
            }

            if (!string.IsNullOrEmpty(patientFilter.LastName))
            {
                set = set.Where(s => s.LastName.Contains(patientFilter.LastName));
            }

            if (!string.IsNullOrEmpty(patientFilter.FirstName))
            {
                set = set.Where(s => s.FirstName.Contains(patientFilter.FirstName));
            }

            if (!string.IsNullOrEmpty(patientFilter.PhoneNumber))
            {
                set = set.Where(s => s.PhoneNumber.Contains(patientFilter.PhoneNumber));
            }

            set = patientFilter.OrderDescending
                ? set.OrderByDescending(GetSortProperty(patientFilter))
                : set.OrderBy(GetSortProperty(patientFilter));
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

        return new PaginatedCollection<Patient>(collection, page, pageSize, totalCount);
    }

    public Task UpdateAsync(Patient patient, CancellationToken cancellationToken)
    {
        context.Update(patient);
        return context.SaveChangesAsync(cancellationToken);
    }

    private static Expression<Func<Patient, object>> GetSortProperty(
        PatientFilter filter)
    {
        return filter.SortColumn switch
        {
            PatientSortColumn.Id => patient => patient.Id,
            PatientSortColumn.FirstName => patient => patient.FirstName,
            PatientSortColumn.LastName => patient => patient.LastName,
            PatientSortColumn.MiddleName => patient => patient.MiddleName,
            PatientSortColumn.Email => patient => patient.Email,
            PatientSortColumn.PhoneNumber => patient => patient.PhoneNumber,
            _ => patient => patient.Id
        };
    }
}