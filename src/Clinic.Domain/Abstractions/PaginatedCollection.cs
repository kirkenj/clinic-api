namespace Clinic.Domain.Abstractions;
public sealed class PaginatedCollection<T>(
    ICollection<T> items,
    int page,
    int pageSize,
    int totalCount)
{
    public ICollection<T> Items { get; } = items;

    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = totalCount;

    public bool HasNextPage => Page * PageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;
}