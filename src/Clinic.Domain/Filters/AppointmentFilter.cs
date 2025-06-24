using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;
public sealed class AppointmentFilter
{
    public ICollection<uint>? Ids { get; set; }
    public ICollection<uint>? DoctorIds { get; set; }
    public ICollection<uint>? PatientIds { get; set; }
    public ICollection<uint>? ServiceIds { get; set; }
    public DateTime? DateTimeStart { get; set; }
    public DateTime? DateTimeEnd { get; set; }
    public bool? IsApprovedByClinic { get; set; }
    public bool? IsApprovedByUser { get; set; }
    public ICollection<AppointmentStatus>? Statuses { get; set; }
    public bool OrderDescending { get; set; } = true;
    public AppointmentSortColumn SortColumn { get; set; } = AppointmentSortColumn.Id;
}