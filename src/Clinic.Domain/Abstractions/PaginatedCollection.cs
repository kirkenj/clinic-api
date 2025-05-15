namespace Clinic.Domain.Abstractions;
public sealed class PaginatedCollection<T>(
    ICollection<T> items,
    uint page,
    uint pageSize,
    uint totalCount)
{
    public ICollection<T> Items { get; } = items;

    public uint Page { get; } = page;
    public uint PageSize { get; } = pageSize;
    public uint TotalCount { get; } = totalCount;

    public bool HasNextPage => Page * PageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;
}