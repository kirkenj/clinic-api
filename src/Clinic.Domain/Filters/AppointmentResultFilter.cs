using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;
public sealed class AppointmentResultFilter
{
    public ICollection<uint>? Ids { get; set; }
    public ICollection<uint>? AppointmentIds { get; set; }
    public AppointmentResultSortColumn SortColumn { get; set; } = AppointmentResultSortColumn.Id;
    public bool OrderDescending { get; set; } = true;
}