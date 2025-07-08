using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;

namespace Clinic.Domain.Repositories;

public interface IServiceRepository
{
    public Task<Service?> GetByIdAsync(uint id, CancellationToken cancellationToken);

    public Task<PaginatedCollection<Service>> GetPaginatedAsync(ServiceFilter? serviceFilter,
        int page, int pageSize,
        CancellationToken cancellationToken);

    public Task AddAsync(Service service, CancellationToken cancellationToken);
    public Task UpdateAsync(Service service, CancellationToken cancellationToken);
    public Task DeleteAsync(Service service, CancellationToken cancellationToken);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}