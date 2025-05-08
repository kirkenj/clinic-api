using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Commands.Delete;

public sealed record class DeleteSpeciallizationCommand(
    uint Id
    ) : IRequest<Result>
{ }