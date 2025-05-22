using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IServiceRepository
{
    public Task<Service?> GetByIdAsync(uint id, CancellationToken cancellationToken);

    public Task<ICollection<Service>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);

    public Task<PaginatedCollection<Service>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken);

    public Task AddAsync(Service service, CancellationToken cancellationToken);
    public Task UpdateAsync(Service service, CancellationToken cancellationToken);
    public Task DeleteAsync(Service service, CancellationToken cancellationToken);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}