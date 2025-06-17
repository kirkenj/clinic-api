using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;

public sealed class OfficeFilter
{
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public bool OrderDescending { get; set; } = true;
    public OfficeSortColumn SortColumn { get; set; } = OfficeSortColumn.Id;
}