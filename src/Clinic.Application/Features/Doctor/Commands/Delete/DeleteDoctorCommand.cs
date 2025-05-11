using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctor.Commands.Delete;

public sealed record DeleteDoctorCommand(uint Id) : IRequest<Result>
{ }