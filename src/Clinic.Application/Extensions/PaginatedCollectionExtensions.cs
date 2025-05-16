using Clinic.Domain.Abstractions;

namespace Clinic.Application.Extensions;

public static class PaginatedCollectionExtensions
{
    public static PaginatedCollection<TDestination> Select<TSourse, TDestination>(
        this PaginatedCollection<TSourse> source,
        Func<TSourse, TDestination> selectDelegate)
    {
        var officeResponses = source.Items.Select(selectDelegate).ToArray();
        return new PaginatedCollection<TDestination>(officeResponses, source.Page,
            source.PageSize, source.TotalCount);
    }
}