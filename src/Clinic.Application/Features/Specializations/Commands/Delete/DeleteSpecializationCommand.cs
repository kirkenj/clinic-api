using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Delete;

public sealed record DeleteSpecializationCommand(uint Id) : IRequest<Result>
{ }