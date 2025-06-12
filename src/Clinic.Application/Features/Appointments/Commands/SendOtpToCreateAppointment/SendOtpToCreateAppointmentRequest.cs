using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.SendOtpToCreateAppointment;

public sealed class SendOtpToCreateAppointmentRequest : IRequest<Result>
{
    public uint AppointmentId { get; set; }
}