using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Update;

public sealed record UpdateSpecializationCommand(
    uint Id,
    string Name
    ) : IRequest<Result>
{ }