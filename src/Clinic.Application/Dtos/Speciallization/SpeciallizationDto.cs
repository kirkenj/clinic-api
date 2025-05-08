namespace Clinic.Application.Dtos.Speciallization;
public sealed class SpeciallizationDto
{
    public uint Id { get; private set; }
    public string Name { get; private set; } = default!;

    public static SpeciallizationDto FromSpeciallization(Domain.Speciallization.Speciallization speciallization) => new()
    {
        Id = speciallization.Id,
        Name = speciallization.Name,
    };
}