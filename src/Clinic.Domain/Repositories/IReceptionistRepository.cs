using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IReceptionistRepository
{
    public Task<Receptionist?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<Receptionist?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    public Task<ICollection<Receptionist>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false);
    public Task<PaginatedCollection<Receptionist>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken);
    public Task UpdateAsync(Receptionist receptionist, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}