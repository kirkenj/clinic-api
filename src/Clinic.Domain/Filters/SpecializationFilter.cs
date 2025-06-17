using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;

public sealed class SpecializationFilter
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public bool OrderDescending { get; set; } = true;
    public SpecializationSortColumn SortColumn { get; set; } = SpecializationSortColumn.Id;
}