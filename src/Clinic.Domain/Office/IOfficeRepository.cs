namespace Clinic.Domain.Office;

public interface IOfficeRepository
{
    public Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<ICollection<Office>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);
    public Task AddAsync(Office obj, CancellationToken cancellationToken);
    public Task UpdateAsync(Office newState, CancellationToken cancellationToken);
    public Task DeleteAsync(Office officeToDelete, CancellationToken cancellationToken);
    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
}