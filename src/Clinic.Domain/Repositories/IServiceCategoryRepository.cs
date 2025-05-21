using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IServiceCategoryRepository
{
    public Task<ServiceCategory?> GetByIdAsync(uint id, CancellationToken cancellationToken);

    public Task<ICollection<ServiceCategory>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false);

    public Task<PaginatedCollection<ServiceCategory>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken);

    public Task AddAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken);
    public Task UpdateAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken);
    public Task DeleteAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}