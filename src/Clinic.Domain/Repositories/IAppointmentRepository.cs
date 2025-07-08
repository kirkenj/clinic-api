using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;

namespace Clinic.Domain.Repositories;

public interface IAppointmentRepository
{
    public Task<Appointment?> GetByIdAsync(uint id, CancellationToken cancellationToken);

    public Task<ICollection<Appointment>> GetAllAsync(CancellationToken cancellationToken, 
        bool includeTracking = false);

    public Task<PaginatedCollection<Appointment>> GetPaginatedAsync(AppointmentFilter? appointmentFilter, 
        int page, int pageSize, CancellationToken cancellationToken);

    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
    public Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken);
    public Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken);

    public Task<bool> ExistsApprovedWithPatientAndDatetimeAsync(uint patientId, DateTime dateTimeStart,
        DateTime dateTimeEnd, CancellationToken cancellationToken);

    public Task<bool> ExistsApprovedWithDoctorAndDatetimeAsync(uint doctorId, DateTime dateTimeStart,
        DateTime dateTimeEnd, CancellationToken cancellationToken);

    public Task<Appointment?> GetAsync(uint patientId, uint doctorId, uint serviceId, DateTime dateTimeStart,
        DateTime dateTimeEnd, CancellationToken cancellationToken);

    public Task<PaginatedCollection<Appointment>> GetPatientsAppointments(uint patientId, int page, int pageSize,
        CancellationToken cancellationToken);

    public Task<PaginatedCollection<Appointment>> GetDoctorsAppointments(uint doctorId, int page, int pageSize,
        CancellationToken cancellationToken);

    public Task<IEnumerable<Appointment>> GetDoctorsAppointmentsOnDate(uint doctorId, DateTime date,
        CancellationToken cancellationToken, AppointmentStatus? appointmentStatus = null);
}