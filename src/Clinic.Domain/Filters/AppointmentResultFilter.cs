namespace Clinic.Domain.Filters;
public sealed class AppointmentResultFilter
{
    public ICollection<uint>? Ids { get; set; }
    public bool OrderDescending { get; set; } = true;
}