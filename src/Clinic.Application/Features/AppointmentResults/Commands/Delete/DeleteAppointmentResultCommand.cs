using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Commands.Delete;

public sealed class DeleteAppointmentResultCommand : IRequest<Result>
{
    public uint Id { get; set; }
}