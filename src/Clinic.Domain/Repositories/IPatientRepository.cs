using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IPatientRepository
{
    public Task<Patient?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<ICollection<Patient>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);

    public Task<PaginatedCollection<Patient>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken);

    public Task AddAsync(Patient patient, CancellationToken cancellationToken);
    public Task UpdateAsync(Patient patient, CancellationToken cancellationToken);
    public Task DeleteAsync(Patient patient, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}