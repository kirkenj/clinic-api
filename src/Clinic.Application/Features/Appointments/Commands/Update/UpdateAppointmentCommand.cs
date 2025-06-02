using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Update;

public sealed class UpdateAppointmentCommand : IRequest<Result>
{
    public uint Id { get; set; }
    public uint PatientId { get; set; }
    public uint DoctorId { get; set; }
    public uint ServiceId { get; set; }
    public DateTime DateTime { get; set; }
    public bool IsConfirmedByUser { get; set; }
    public bool IsConfirmedByClinic { get; set; }
    public AppointmentStatus Status { get; set; }

}