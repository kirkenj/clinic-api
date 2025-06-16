using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IDoctorRepository
{
    public Task<Doctor?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<Doctor?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    public Task<ICollection<Doctor>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);
    public Task<PaginatedCollection<Doctor>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken);
    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken);
    public Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken);
    public Task DeleteAsync(Doctor doctor, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}