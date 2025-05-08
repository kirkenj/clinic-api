namespace Clinic.Domain.Office;

public interface IOfficeRepository
{
    public Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<ICollection<Office>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);
    public Task AddAsync(Office office, CancellationToken cancellationToken);
    public Task UpdateAsync(Office office, CancellationToken cancellationToken);
    public Task DeleteAsync(Office office, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
}