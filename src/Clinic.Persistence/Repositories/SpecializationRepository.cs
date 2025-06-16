using System.Linq.Expressions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public class SpecializationRepository(ClinnicContext context) : ISpecializationRepository
{
    private readonly DbSet<Specialization> _dbSet = context.Set<Specialization>();

    public async Task<PaginatedCollection<Specialization>> GetPaginatedAsync(
        SpecializationFilter? filter,
        int page, int pageSize, 
        CancellationToken cancellationToken)
    {
        var set = _dbSet.AsQueryable();
        if (filter is not null)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                set = set.Where(s => s.Name.Contains(filter.Name));
            }

            if (filter.IsActive is not null)
            {
                set = set.Where(s => s.IsActive);
            }

            set = filter.SortOrder == SortOrder.Descending
                ? set.OrderByDescending(GetSortProperty(filter))
                : set.OrderBy(GetSortProperty(filter));
        }

        var totalCount = await set.CountAsync(cancellationToken);

        var collection = await set.OrderBy(o => o.Id)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Specialization>(collection, page, pageSize, totalCount);
    }

    public async Task AddAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        _dbSet.Add(specialization);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        context.Remove(specialization);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<Specialization?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        context.Update(specialization);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static Expression<Func<Specialization, object>> GetSortProperty
        (SpecializationFilter filter)
    {
        return filter.SortColumn switch
        {
            SpecializationSortColumn.Id => specialization => specialization.Id,
            SpecializationSortColumn.Name => specialization => specialization.Name,
            SpecializationSortColumn.IsActive => specialization => specialization.IsActive,
            _ => specialization => specialization.Id
        };
    }

    public async Task<ICollection<Specialization>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }
}