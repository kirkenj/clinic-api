using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Add.Patient;

public sealed class PatientAddAppointmentCommand : IRequest<Result<string>>
{
    public uint DoctorId { get; set; }
    public uint ServiceId { get; set; }
    public DateTime DateTime { get; set; }
}