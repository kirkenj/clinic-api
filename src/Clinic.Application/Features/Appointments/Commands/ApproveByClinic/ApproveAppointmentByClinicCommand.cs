using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveByClinic;

public sealed class ApproveAppointmentByClinicCommand : IRequest<Result>
{
    public uint AppointmentId { get; init; }
}