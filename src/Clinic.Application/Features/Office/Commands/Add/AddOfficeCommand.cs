using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Add;

public sealed record class AddOfficeCommand(
    string Address,
    string PhoneNumber
    ) : IRequest<Result>
{ }