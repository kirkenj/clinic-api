namespace Clinic.Application.Responses.Specialization;
public sealed record SpecializationResponse(
    uint Id,
    string Name,
    bool IsActive
    )
{
    public static SpecializationResponse FromSpecialization(Domain.Specialization.Specialization specialization) =>
        new(specialization.Id, specialization.Name, specialization.IsActive);

    public static IEnumerable<SpecializationResponse> FromSpecializationCollection(IEnumerable<Domain.Specialization.Specialization> specializations) =>
            specializations.Select(FromSpecialization);
}