using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;

namespace Clinic.Domain.Repositories;

public interface IOfficeRepository
{
    public Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken);

    public Task<PaginatedCollection<Office>> GetPaginatedAsync(OfficeFilter? officeFilter, 
        int page, int pageSize,
        CancellationToken cancellationToken);

    public Task AddAsync(Office office, CancellationToken cancellationToken);
    public Task UpdateAsync(Office office, CancellationToken cancellationToken);
    public Task DeleteAsync(Office office, CancellationToken cancellationToken);
    public Task<bool> ExistsByAddressAsync(string address, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
}