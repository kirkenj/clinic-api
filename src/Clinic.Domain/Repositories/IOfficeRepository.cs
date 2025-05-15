using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IOfficeRepository
{
    public Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<ICollection<Office>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);
    public Task<ICollection<Office>> GetPaginatedAsync(uint page, uint pageSize, CancellationToken cancellationToken, bool includeTracking = false);
    public Task AddAsync(Office office, CancellationToken cancellationToken);
    public Task UpdateAsync(Office office, CancellationToken cancellationToken);
    public Task DeleteAsync(Office office, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
}