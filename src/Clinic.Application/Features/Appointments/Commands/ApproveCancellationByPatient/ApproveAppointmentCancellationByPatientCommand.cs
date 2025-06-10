using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;

public sealed class ApproveAppointmentCancellationByPatientCommand : IRequest<Result>
{
    public uint AppointmentId { get; init; }
    public uint PatientId { get; init; }
    public string Otp { get; set; } = default!;

    public void TrimStringProperties()
    {
        Otp = Otp.Trim();
    }
}