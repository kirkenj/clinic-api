using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Commands.Add;

public sealed record class AddSpeciallizationCommand(
    string Name
    ) : IRequest<Result>
{ }