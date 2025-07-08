using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated.Patient;
public sealed class PatientAppointmentFilter
{
    public ICollection<uint>? Ids { get; set; }
    public ICollection<uint>? DoctorIds { get; set; }
    public ICollection<uint>? ServiceIds { get; set; }
    public DateTime? DateTimeStart { get; set; }
    public DateTime? DateTimeEnd { get; set; }
    public bool? IsApprovedByClinic { get; set; }
    public bool? IsApprovedByUser { get; set; }
    public ICollection<AppointmentStatus>? Statuses { get; set; }
    public bool OrderDescending { get; set; } = true;
    public AppointmentSortColumn SortColumn { get; set; } = AppointmentSortColumn.Id;

    public AppointmentFilter ToAppointmentFilter()
    {
        return new AppointmentFilter
        {
            Ids = Ids,
            DoctorIds = DoctorIds,
            ServiceIds = ServiceIds,
            DateTimeStart = DateTimeStart,
            DateTimeEnd = DateTimeEnd,
            IsApprovedByClinic = IsApprovedByClinic,
            IsApprovedByUser = IsApprovedByUser,
            Statuses = Statuses,
            OrderDescending = OrderDescending,
            SortColumn = SortColumn
        };
    }
}