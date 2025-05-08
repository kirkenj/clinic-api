using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specialization.Commands.Add;

public sealed record AddSpecializationCommand(string Name) : IRequest<Result>
{ }