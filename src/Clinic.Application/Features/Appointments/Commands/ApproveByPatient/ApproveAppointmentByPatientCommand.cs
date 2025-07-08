using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveByPatient;

public sealed class ApproveAppointmentByPatientCommand : IRequest<Result>
{
    public uint AppointmentId { get; set; }
    public string Otp { get; set; } = default!;

    public void TrimStringProperties()
    {
        Otp = Otp.Trim();
    }
}