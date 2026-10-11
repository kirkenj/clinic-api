using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;

namespace Clinic.Domain.Repositories;

public interface IAppointmentRepository
{
    public Task<Appointment?> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<PaginatedCollection<Appointment>> GetPaginatedAsync(AppointmentFilter? appointmentFilter,
        int page, int pageSize, CancellationToken cancellationToken);

    public Task<IEnumerable<Appointment>> GetAppointmentsToOutdate(DateTime dateTime, CancellationToken cancellationToken);

    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
    public Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken);
    public Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken);
    public Task UpdateRangeAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken);

    public Task<IEnumerable<Appointment>> GetAppointmentsOnTime(DateTime dateTime, uint doctorId, 
        IEnumerable<AppointmentStatus> statuses, CancellationToken cancellationToken);
    
    public Task<bool> ExistsApprovedWithPatientAndDatetimeAsync(uint patientId, DateTime dateTimeStart,
        DateTime dateTimeEnd, CancellationToken cancellationToken);

    public Task<bool> ExistsApprovedWithDoctorAndDatetimeAsync(uint doctorId, DateTime dateTimeStart,
        DateTime dateTimeEnd, CancellationToken cancellationToken);

    public Task<Appointment?> GetAsync(uint patientId, uint doctorId, uint serviceId, DateTime dateTimeStart,
        DateTime dateTimeEnd, CancellationToken cancellationToken);

    public Task<IEnumerable<Appointment>> GetDoctorsAppointmentsOnDate(uint doctorId, DateTime date,
        CancellationToken cancellationToken, AppointmentStatus? appointmentStatus = null);
}