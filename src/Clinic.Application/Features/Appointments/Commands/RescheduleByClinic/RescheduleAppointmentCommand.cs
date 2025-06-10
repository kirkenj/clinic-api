using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.RescheduleByClinic;

public sealed class RescheduleAppointmentCommand : IRequest<Result>
{
    public uint AppointmentId { get; init; }
    public DateTime NewDateTime { get; init; }
    public uint NewDoctorId { get; init; }
}