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
                set = set.Where(s => s.IsActive == filter.IsActive.Value);
            }

            set = filter.OrderDescending
                ? set.OrderByDescending(GetSortProperty(filter))
                : set.OrderBy(GetSortProperty(filter));
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

        return new PaginatedCollection<Specialization>(collection, page, pageSize, totalCount);
    }

    public Task AddAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        _dbSet.Add(specialization);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        context.Remove(specialization);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(s => EF.Functions.ILike(s.Name, name), cancellationToken);
    }

    public async Task<Specialization?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        context.Update(specialization);
        return context.SaveChangesAsync(cancellationToken);
    }

    private static Expression<Func<Specialization, object>> GetSortProperty(
        SpecializationFilter filter)
    {
        return filter.SortColumn switch
        {
            SpecializationSortColumn.Id => specialization => specialization.Id,
            SpecializationSortColumn.Name => specialization => specialization.Name,
            SpecializationSortColumn.IsActive => specialization => specialization.IsActive,
            _ => specialization => specialization.Id
        };
    }
}