using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Specializations;

public sealed record SpecializationResponse(
    uint Id,
    string Name
)
{
    public static SpecializationResponse FromSpecialization(Specialization specialization)
    {
        return new SpecializationResponse(
            specialization.Id,
            specialization.Name
        );
    }
}