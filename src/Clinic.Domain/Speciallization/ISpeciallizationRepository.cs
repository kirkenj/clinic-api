namespace Clinic.Domain.Speciallization;
public interface ISpeciallizationRepository
{
    public Task<Speciallization?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<ICollection<Speciallization>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);
    public Task AddAsync(Speciallization obj, CancellationToken cancellationToken);
    public Task UpdateAsync(Speciallization newState, CancellationToken cancellationToken);
    public Task DeleteAsync(Speciallization obj, CancellationToken cancellationToken);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}