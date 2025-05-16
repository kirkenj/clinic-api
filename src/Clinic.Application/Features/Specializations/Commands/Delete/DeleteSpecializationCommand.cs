using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Delete;

public sealed class DeleteSpecializationCommand : IRequest<Result>
{
    public uint Id { get; set; }
}