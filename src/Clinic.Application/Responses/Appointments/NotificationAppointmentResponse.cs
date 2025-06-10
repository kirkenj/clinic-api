using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Appointments;

public sealed record NotificationAppointmentResponse(
    string DoctorName,
    string ServiceName,
    decimal Price,
    DateTime DateTime)
{
    public static NotificationAppointmentResponse Create(Doctor doctor, DateTime slotStart, Service service)
    {
        return new NotificationAppointmentResponse(doctor.FirstName + " " + doctor.LastName,
            service.Name, service.Price, slotStart);
    }
}