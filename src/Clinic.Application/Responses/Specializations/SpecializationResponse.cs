using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Specializations;
public sealed record SpecializationResponse(
    uint Id,
    string Name,
    bool IsActive
    )
{
    public static SpecializationResponse FromSpecialization(Specialization specialization) =>
        new(
            specialization.Id,
            specialization.Name,
            specialization.IsActive
            );

    public static IEnumerable<SpecializationResponse> FromSpecializationCollection(IEnumerable<Specialization> specializations) =>
            specializations.Select(FromSpecialization);
}