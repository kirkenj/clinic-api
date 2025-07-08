using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Clinic.Persistence.Repositories;

public class ReceptionistRepository(ClinnicContext context) : IReceptionistRepository
{
    private readonly DbSet<Receptionist> _dbSet = context.Set<Receptionist>();

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(r => EF.Functions.ILike(r.PhoneNumber, phoneNumber), cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(x => EF.Functions.ILike(x.Email, email), cancellationToken);
    }

    public Task<Receptionist?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbSet.FirstOrDefaultAsync(x => EF.Functions.ILike(x.Email, email), cancellationToken);
    }

    public async Task<ICollection<Receptionist>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Receptionist?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Receptionist>> GetPaginatedAsync(ReceptionistFilter? receptionistFilter, 
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var set = _dbSet.AsQueryable().Where(d => d.IsActive);
        if (receptionistFilter is not null)
        {
            if (receptionistFilter.Ids is not null && receptionistFilter.Ids.Count != 0)
            {
                set = set.Where(o => receptionistFilter.Ids.Contains(o.Id));
            }

            if (!string.IsNullOrEmpty(receptionistFilter.Email))
            {
                set = set.Where(s => s.Email.Contains(receptionistFilter.Email));
            }

            if (!string.IsNullOrEmpty(receptionistFilter.MiddleName))
            {
                set = set.Where(s => s.MiddleName.Contains(receptionistFilter.MiddleName));
            }

            if (!string.IsNullOrEmpty(receptionistFilter.LastName))
            {
                set = set.Where(s => s.LastName.Contains(receptionistFilter.LastName));
            }

            if (!string.IsNullOrEmpty(receptionistFilter.FirstName))
            {
                set = set.Where(s => s.FirstName.Contains(receptionistFilter.FirstName));
            }

            if (!string.IsNullOrEmpty(receptionistFilter.PhoneNumber))
            {
                set = set.Where(s => s.PhoneNumber.Contains(receptionistFilter.PhoneNumber));
            }

            set = receptionistFilter.OrderDescending
                ? set.OrderByDescending(GetSortProperty(receptionistFilter))
                : set.OrderBy(GetSortProperty(receptionistFilter));
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


        return new PaginatedCollection<Receptionist>(collection, page, pageSize, totalCount);
    }

    private Expression<Func<Receptionist, object>> GetSortProperty(ReceptionistFilter receptionistFilter)
    {
        return receptionistFilter.SortColumn switch
        {
            ReceptionistSortColumn.Id => Receptionist => Receptionist.Id,
            ReceptionistSortColumn.FirstName => Receptionist => Receptionist.FirstName,
            ReceptionistSortColumn.LastName => Receptionist => Receptionist.LastName,
            ReceptionistSortColumn.MiddleName => Receptionist => Receptionist.MiddleName,
            ReceptionistSortColumn.Email => Receptionist => Receptionist.Email,
            ReceptionistSortColumn.PhoneNumber => Receptionist => Receptionist.PhoneNumber,
            _ => patient => patient.Id
        };
    }

    public Task UpdateAsync(Receptionist receptionist, CancellationToken cancellationToken)
    {
        context.Update(receptionist);
        return context.SaveChangesAsync(cancellationToken);
    }
}