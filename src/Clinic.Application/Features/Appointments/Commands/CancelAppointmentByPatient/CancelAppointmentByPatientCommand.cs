using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelAppointmentByPatient;

public sealed class CancelAppointmentByPatientCommand : IRequest<Result<string>>
{
    public uint AppointmentId { get; set; }
}