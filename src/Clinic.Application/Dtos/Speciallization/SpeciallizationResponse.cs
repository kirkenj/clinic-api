namespace Clinic.Application.Dtos.Speciallization;
public sealed record class SpeciallizationResponse(
    uint Id,
    string Name,
    bool IsActive
    )
{
    public static SpeciallizationResponse FromSpeciallization(Domain.Speciallization.Speciallization speciallization) =>
        new(speciallization.Id, speciallization.Name, speciallization.IsActive);

    public static IEnumerable<SpeciallizationResponse> FromSpeciallizationCollection(IEnumerable<Domain.Speciallization.Speciallization> speciallizations) =>
            speciallizations.Select(FromSpeciallization);
}