using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Appointments;

public sealed record AppointmentResponse(
    uint Id,
    uint PatientId,
    uint DoctorId,
    uint ServiceId,
    DateTime DateTime,
    string Status
    )
{
    public static AppointmentResponse FromAppointment(Appointment appointment)
    {
        return new AppointmentResponse(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.ServiceId,
            appointment.DateTimeStart, 
            appointment.Status.ToString()
        );
    }
}