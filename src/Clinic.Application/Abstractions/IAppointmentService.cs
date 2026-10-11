using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;

namespace Clinic.Application.Abstractions;
public interface IAppointmentService
{
    public Task<Result<string>> Add(uint patientId, uint doctorId, uint serviceId,
        DateTime dateTime, CancellationToken cancellationToken);
    public Task<Result> ApproveCancellationByPatient(uint appointmentId, string otp,
        CancellationToken cancellationToken);
    public Task<Result> ApproveByClinicAsync(uint appointmentId, CancellationToken cancellationToken);
    public Task<Result> ApproveByPatientAsync(uint appointmentId, string otp, CancellationToken cancellationToken);
    public Task<Result> CancelByClinicAsync(uint appointmentId, CancellationToken cancellationToken);
    public Task<Result<string>> CancelByPatientAsync(uint appointmentId, CancellationToken cancellationToken);
    public Task<Result<AppointmentResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<Result<IEnumerable<DateTime>>> GetDoctorsAvailableSlotsOnDate(DateTime date, uint doctorId, CancellationToken cancellationToken);
    public Task<Result<PaginatedCollection<AppointmentResponse>>> GetPaginatedAsync(AppointmentFilter? appointmentFilter, int page, int pageSize, CancellationToken cancellationToken);
    public Task<Result> RescheduleAppointmentAsync(uint appointmentId, uint newDoctorId,
        DateTime newDateTime, CancellationToken cancellationToken);
    public Task<Result> SendOtpToCreateAppointment(uint appointmentId, CancellationToken cancellationToken);
}
