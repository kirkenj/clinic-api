using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Update;

public sealed record UpdateDoctorCommand(
    uint Id,
    string FirstName,
    string LastName,
    string MiddleName,
    string Email,
    string PhoneNumber,
    uint SpecializationId,
    uint OfficeId
    ) : IRequest<Result>
{ }