using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Delete;

public sealed record DeleteDoctorCommand(uint Id) : IRequest<Result>
{ }