using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.Update;

public sealed record UpdateReceptionistCommand(
    uint Id,
    string FirstName,
    string LastName,
    string MiddleName,
    string Email,
    string PhoneNumber
    ) : IRequest<Result>
{ }