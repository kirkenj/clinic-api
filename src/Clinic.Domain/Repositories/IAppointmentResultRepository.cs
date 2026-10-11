using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;

namespace Clinic.Domain.Repositories;

public interface IAppointmentResultRepository
{
    public Task<AppointmentResult?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<AppointmentResult?> GetByAppointmentIdAsync(uint appointmentId, CancellationToken cancellationToken);
    public Task<bool> ExistsByAppointmentIdAsync(uint appointmentId, CancellationToken cancellationToken);
    public Task<PaginatedCollection<AppointmentResult>> GetPaginatedAsync(AppointmentResultFilter? appointmentResultFilter,
        int page, int pageSize, CancellationToken cancellationToken);
    public Task<PaginatedCollection<AppointmentResult>> GetPaginatedAsync(uint patientId, 
        AppointmentResultFilter? appointmentResultFilter, int page, int pageSize, CancellationToken cancellationToken);
    public Task AddAsync(AppointmentResult appointmentResult, CancellationToken cancellationToken);
    public Task UpdateAsync(AppointmentResult appointmentResult, CancellationToken cancellationToken);
    public Task DeleteAsync(AppointmentResult appointmentResult, CancellationToken cancellationToken);
}