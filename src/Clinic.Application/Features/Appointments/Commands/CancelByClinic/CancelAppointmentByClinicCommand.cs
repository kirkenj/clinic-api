using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelByClinic;

public sealed class CancelAppointmentByClinicCommand : IRequest<Result>
{
    public uint Id { get; init; }
}