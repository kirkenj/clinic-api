using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;

public sealed class DoctorFilter
{
    public ICollection<uint>? Ids { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public ICollection<uint>? SpecializationIds { get; set; }
    public ICollection<uint>? OfficeIds { get; set; }
    public bool? IsActive { get; set; }
    public bool OrderDescending { get; set; } = true;
    public DoctorSortColumn SortColumn { get; set; } = DoctorSortColumn.Id;
}