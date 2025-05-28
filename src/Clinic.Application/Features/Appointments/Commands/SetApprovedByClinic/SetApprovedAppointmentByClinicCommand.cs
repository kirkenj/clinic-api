using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByClinic;

public sealed class SetApprovedAppointmentByClinicCommand : IRequest<Result>
{
    public uint AppointmentId { get; set; }
    public bool IsApproved { get; set; }
}