using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Delete;

public sealed class DeleteAppointmentCommand : IRequest<Result>
{
    public uint Id { get; set; }
}