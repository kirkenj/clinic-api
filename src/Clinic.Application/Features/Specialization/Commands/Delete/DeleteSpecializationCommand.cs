using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specialization.Commands.Delete;

public sealed record DeleteSpecializationCommand(uint Id) : IRequest<Result>
{ }