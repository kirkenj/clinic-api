using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.Delete;

public sealed class DeletePatientCommand : IRequest<Result>
{
    public uint Id { get; set; }
}