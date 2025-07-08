using Clinic.Domain.Abstractions;

namespace Clinic.Application.Abstractions;
public interface IUserAccessService
{
    public Task<Result<(uint Id, RoleType Role)>> ValidateUserAccessAsync(IEnumerable<RoleType> availableRoles,
        CancellationToken cancellationToken);
}
