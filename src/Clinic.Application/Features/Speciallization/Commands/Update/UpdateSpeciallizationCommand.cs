using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Commands.Update;

public sealed record class UpdateSpeciallizationCommand(
    uint Id,
    string Name
    ) : IRequest<Result>
{ }