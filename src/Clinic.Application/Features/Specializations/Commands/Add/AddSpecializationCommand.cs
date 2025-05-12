using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Add;

public sealed record AddSpecializationCommand(string Name) : IRequest<Result>
{ }