namespace Clinic.Domain.Office;

public interface IOfficeRepository
{
    public Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken = default);
    public Task<ICollection<Office>> GetAllAsync(bool asTracking = false, CancellationToken cancellationToken = default);
    public Task AddAsync(Office obj, CancellationToken cancellationToken = default);
    public Task UpdateAsync(Office newState, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Office officeToDelete, CancellationToken cancellationToken = default);
    public Task<bool> ExcistsByAddressAsync(string address, CancellationToken cancellationToken = default);
}