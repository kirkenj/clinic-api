using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Add;

public sealed record AddOfficeCommand(
    string Address,
    string PhoneNumber
    ) : IRequest<Result>
{ }