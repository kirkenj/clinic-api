using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Clinic.Infrastructure.Services.HttpUserContextService;
public class UserAccessService(
    IHttpContextAccessor httpContextAccessor,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    IReceptionistRepository receptionistRepository)
    : IUserAccessService
{
    public async Task<Result<ValueTuple<uint, RoleType>>> ValidateUserAccessAsync(IEnumerable<RoleType> availableRoles,
        CancellationToken cancellationToken)
    {
        var contextUser = httpContextAccessor?.HttpContext?.User
            ?? throw new Exception("No http context's user");

        var roleClaim = contextUser.FindFirst(t => t.Type.Equals(ClaimTypes.Role));
        if (!Enum.TryParse<RoleType>(roleClaim?.Value, out var role))
        {
            return Result.Failure<ValueTuple<uint, RoleType>>(BasicErrors.InvalidToken());
        }

        if (!availableRoles.Contains(role))
        {
            return Result.Failure<ValueTuple<uint, RoleType>>(BasicErrors.NoAccess());
        }

        var idClaim = contextUser.FindFirst(t => t.Type.Equals(ClaimTypes.NameIdentifier));
        if (!uint.TryParse(idClaim?.Value, out var id))
        {
            return Result.Failure<ValueTuple<uint, RoleType>>(BasicErrors.InvalidToken());
        }

        switch (role)
        {
            case RoleType.Patient:
                {
                    var patient = await patientRepository.GetByIdAsync(id, cancellationToken);
                    if (patient == null || !patient.IsActive)
                    {
                        return Result.Failure<ValueTuple<uint, RoleType>>(BasicErrors.NoAccess());
                    }
                    break;
                }
            case RoleType.Doctor:
                {
                    var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken);
                    if (doctor == null || !doctor.IsActive)
                    {
                        return Result.Failure<ValueTuple<uint, RoleType>>(BasicErrors.NoAccess());
                    }
                    break;
                }
            case RoleType.Receptionist:
                {
                    var receptionist = await receptionistRepository.GetByIdAsync(id, cancellationToken);
                    if (receptionist == null || !receptionist.IsActive)
                    {
                        return Result.Failure<ValueTuple<uint, RoleType>>(BasicErrors.NoAccess());
                    }
                    break;
                }
        }

        return (id, role);
    }
}
