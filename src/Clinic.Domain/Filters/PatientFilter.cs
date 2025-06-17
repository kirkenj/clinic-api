using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;

public sealed class PatientFilter
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public bool OrderDescending { get; set; } = true;
    public PatientSortColumn SortColumn { get; set; } = PatientSortColumn.Id;
}