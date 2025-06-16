using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;

public sealed class SpecializationFilter
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
    public SpecializationSortColumn SortColumn { get; set; } = SpecializationSortColumn.Id;
}