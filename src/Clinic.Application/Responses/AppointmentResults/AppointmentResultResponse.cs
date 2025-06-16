using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.AppointmentResults;

public sealed record AppointmentResultResponse(
    uint Id,
    string Complaints,
    string Conclusion,
    string Recommendations,
    uint AppointmentId
)
{
    public static AppointmentResultResponse FromAppointmentResult(AppointmentResult appointmentResult)
    {
        return new AppointmentResultResponse(
            appointmentResult.Id,
            appointmentResult.Complaints,
            appointmentResult.Conclusion,
            appointmentResult.Recommendations,
            appointmentResult.AppointmentId
        );
    }
}