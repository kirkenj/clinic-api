using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByPatient;

public sealed class SetApprovedAppointmentByPatientCommand : IRequest<Result>
{
    public uint AppointmentId { get; set; }
    public bool IsBooked { get; set; }
}