using System.Linq.Expressions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class ServiceRepository(ClinnicContext context) : IServiceRepository
{
    private readonly DbSet<Service> _dbSet = context.Set<Service>();

    public async Task<PaginatedCollection<Service>> GetPaginatedAsync(ServiceFilter? serviceFilter, 
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var set = _dbSet.AsQueryable();
        if (serviceFilter is not null)
        {
            if (serviceFilter.Ids is not null && serviceFilter.Ids.Count != 0)
            {
                set = set.Where(o => serviceFilter.Ids.Contains(o.Id));
            }

            if (!string.IsNullOrEmpty(serviceFilter.Name))
            {
                set = set.Where(s => s.Name.Contains(serviceFilter.Name));
            }

            if (serviceFilter.IsActive.HasValue)
            {
                set = set.Where(s => s.IsActive == serviceFilter.IsActive.Value);
            }

            if (serviceFilter.PriceStart.HasValue)
            {
                set = set.Where(s => s.Price >= serviceFilter.PriceStart);
            }

            if (serviceFilter.PriceEnd.HasValue)
            {
                set = set.Where(s => s.Price <= serviceFilter.PriceEnd);
            }

            if (serviceFilter.SpecializationIds is not null && 
                serviceFilter.SpecializationIds.Count != 0)
            {
                set = set.Where(s => serviceFilter.SpecializationIds
                    .Contains(s.SpecializationId));
            }

            set = serviceFilter.OrderDescending
                ? set.OrderByDescending(GetSortProperty(serviceFilter))
                : set.OrderBy(GetSortProperty(serviceFilter));
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

        return new PaginatedCollection<Service>(collection, page, pageSize, totalCount);
    }

    public Task AddAsync(Service service, CancellationToken cancellationToken)
    {
        context.Add(service);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Service service, CancellationToken cancellationToken)
    {
        context.Remove(service);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(d => EF.Functions.ILike(d.Name, name), cancellationToken);
    }

    public async Task<Service?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public Task UpdateAsync(Service service, CancellationToken cancellationToken)
    {
        context.Update(service);
        return context.SaveChangesAsync(cancellationToken);
    }

    private static Expression<Func<Service, object>> GetSortProperty(
        ServiceFilter filter)
    {
        return filter.SortColumn switch
        {
            ServiceSortColumn.Id => service => service.Id,
            ServiceSortColumn.Name => service => service.Name,
            ServiceSortColumn.Price => service => service.Price,
            ServiceSortColumn.SpecializationId => service => service.SpecializationId,
            ServiceSortColumn.IsActive => service => service.IsActive,
            _ => service => service.Id
        };
    }
}