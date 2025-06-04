namespace Clinic.Application.Responses.Appointments;

public sealed record EmailAppointmentResponse(
    string DoctorName, 
    string ServiceName, 
    decimal Price, 
    DateTime DateTime)
{
}