using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;

public sealed class ApproveAppointmentCancellationByPatientCommand : IRequest<Result>
{
    public uint AppointmentId { get; set; }
    public uint PatientId { get; set; }
    public string Otp { get; set; } = default!;

    public void TrimStringProperties()
    {
        Otp = Otp.Trim();
    }
}