using Clinic.Domain.Abstractions;

namespace Clinic.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(uint memberId, RoleType roleTypes);
}