using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Delete;

public sealed record DeleteOfficeCommand(uint Id) : IRequest<Result>
{ }