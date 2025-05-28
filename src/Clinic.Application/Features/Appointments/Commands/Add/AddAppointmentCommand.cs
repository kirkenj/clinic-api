using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Add;

public sealed class AddAppointmentCommand : IRequest<Result>
{
    public uint PatientId { get; set; }
    public uint DoctorId { get; set; }
    public uint ServiceId { get; set; }
    public DateTime DateTime { get; set; }
}