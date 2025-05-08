using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specialization.Commands.Update;

public sealed record UpdateSpecializationCommand(
    uint Id,
    string Name
    ) : IRequest<Result>
{ }