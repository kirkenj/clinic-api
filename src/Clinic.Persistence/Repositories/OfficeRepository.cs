using System.Linq.Expressions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class OfficeRepository(ClinnicContext context) : IOfficeRepository
{
    private readonly DbSet<Office> _dbSet = context.Set<Office>();

    public Task AddAsync(Office office, CancellationToken cancellationToken)
    {
        context.Add(office);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Office office, CancellationToken cancellationToken)
    {
        context.Remove(office);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(d => EF.Functions.ILike(d.PhoneNumber, phoneNumber), cancellationToken);
    }

    public Task<bool> ExistsByAddressAsync(string address, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(d => EF.Functions.ILike(d.Address, address), cancellationToken);
    }

    public async Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Office>> GetPaginatedAsync(OfficeFilter? officeFilter, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var set = _dbSet.AsQueryable().Where(d => d.IsActive);
        if (officeFilter is not null)
        {
            if (officeFilter.Ids is not null && officeFilter.Ids.Count != 0)
            {
                set = set.Where(o => officeFilter.Ids.Contains(o.Id));
            }

            if (!string.IsNullOrEmpty(officeFilter.Address))
            {
                set = set.Where(x => EF.Functions.ILike(x.Address, $"%{officeFilter.Address}%"));
            }

            if (!string.IsNullOrEmpty(officeFilter.PhoneNumber))
            {
                set = set.Where(s => s.PhoneNumber.Contains(officeFilter.PhoneNumber));
            }

            set = officeFilter.OrderDescending
                ? set.OrderByDescending(GetSortProperty(officeFilter))
                : set.OrderBy(GetSortProperty(officeFilter));
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

        return new PaginatedCollection<Office>(collection, page, pageSize, totalCount);
    }

    private static Expression<Func<Office, object>> GetSortProperty(
        OfficeFilter filter)
    {
        return filter.SortColumn switch
        {
            OfficeSortColumn.Id => specialization => specialization.Id,
            OfficeSortColumn.Address => specialization => specialization.Address,
            OfficeSortColumn.PhoneNumber => specialization => specialization.PhoneNumber,
            _ => specialization => specialization.Id
        };
    }

    public Task UpdateAsync(Office office, CancellationToken cancellationToken)
    {
        context.Update(office);
        return context.SaveChangesAsync(cancellationToken);
    }
}