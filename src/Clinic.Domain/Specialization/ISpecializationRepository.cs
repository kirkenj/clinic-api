namespace Clinic.Domain.Specialization;
public interface ISpecializationRepository
{
    public Task<Specialization?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<ICollection<Specialization>> GetAllAsync(CancellationToken cancellationToken, bool includeTracking = false);
    public Task AddAsync(Specialization specialization, CancellationToken cancellationToken);
    public Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken);
    public Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}