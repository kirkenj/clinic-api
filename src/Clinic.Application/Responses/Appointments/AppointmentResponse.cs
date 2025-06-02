using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Appointments;

public sealed record AppointmentResponse(
    uint Id,
    uint PatientId,
    uint DoctorId,
    uint ServiceId,
    bool IsBooked,
    bool IsApproved
)
{
    public static AppointmentResponse FromAppointment(Appointment appointment)
    {
        return new AppointmentResponse(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.ServiceId,
            appointment.IsApprovedByUser,
            appointment.IsApprovedByClinic
        );
    }

    public static IEnumerable<AppointmentResponse> FromAppointmentCollection(IEnumerable<Appointment> appointments)
    {
        return appointments.Select(FromAppointment);
    }
}