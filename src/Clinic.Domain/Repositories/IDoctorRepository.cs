using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IDoctorRepository
{
    public Task<Doctor?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<ICollection<Doctor>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);
    public Task AddAsync(Doctor Doctor, CancellationToken cancellationToken);
    public Task UpdateAsync(Doctor Doctor, CancellationToken cancellationToken);
    public Task DeleteAsync(Doctor Doctor, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}