using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface ISpecializationRepository
{
    public Task<Specialization?> GetByIdAsync(uint id, CancellationToken cancellationToken);

    public Task<ICollection<Specialization>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false);

    public Task<PaginatedCollection<Specialization>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken);

    public Task AddAsync(Specialization specialization, CancellationToken cancellationToken);
    public Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken);
    public Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}