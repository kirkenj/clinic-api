using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctor.Commands.Add;

public sealed record AddDoctorCommand(
    string FirstName, 
    string LastName,
    string MiddleName,
    string Email,
    string PhoneNumber,
    uint SpecializationId,
    uint OfficeId
    ) : IRequest<Result>
{ } 