using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.RescheduleByClinic;

public sealed class RescheduleAppointmentCommand : IRequest<Result>
{
    public uint AppointmentId { get; set; }
    public DateTime NewDateTime { get; set; }
    public uint NewDoctorId { get; set; }
}