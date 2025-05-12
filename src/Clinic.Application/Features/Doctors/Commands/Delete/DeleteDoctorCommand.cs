using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Delete;

public sealed class DeleteDoctorCommand : IRequest<Result>
{
    public uint Id { get; set; }
}