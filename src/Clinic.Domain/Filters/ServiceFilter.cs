using Clinic.Domain.Filters.Sorting;

namespace Clinic.Domain.Filters;

public sealed class ServiceFilter
{
    public bool? IsActive { get; set; }
    public string? Name { get; set; }
    public decimal? PriceStart { get; set; }
    public decimal? PriceEnd { get; set; }
    public ICollection<uint>? SpecializationIds { get; set; } = null;
    public bool OrderDescending { get; set; } = true;
    public ServiceSortColumn SortColumn { get; set; } = ServiceSortColumn.Id;
}