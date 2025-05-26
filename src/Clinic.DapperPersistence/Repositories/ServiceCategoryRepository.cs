using Clinic.DapperPersistence.Absctractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Dapper;

namespace Clinic.DapperPersistence.Repositories;

public sealed class ServiceCategoryRepository(ISqlConnectionFactory sqlConnectionFactory) : IServiceCategoryRepository
{
    public async Task<ServiceCategory?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryFirstOrDefaultAsync<ServiceCategory>
            ("""SELECT * FROM "ServiceCategories" where "Id" = @id""", new { id = (int)id });
        return result;
    }

    public async Task<ICollection<ServiceCategory>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryAsync<ServiceCategory>
            ("""SELECT * FROM "ServiceCategories";""", cancellationToken);
        return result.ToList();
    }

    public async Task<PaginatedCollection<ServiceCategory>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var resultSet = await sqlConnection.QueryAsync<ServiceCategory>
        ("""SELECT * FROM public."ServiceCategories" offset @offset limit @limit""",
            new { limit = pageSize, offset = (page - 1) * pageSize });
        var totalCount = await sqlConnection.QueryFirstAsync<int>
            ("""SELECT COUNT(*) FROM public."ServiceCategories" """);


        return new PaginatedCollection<ServiceCategory>(resultSet, page, pageSize, totalCount);
    }

    public async Task AddAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         INSERT INTO "ServiceCategories" ("Name", 
         "Duration", "IsActive", "CreatedOnUtc") 
         VALUES(@Name, @Duration, @IsActive, @CreatedOnUtc);
         """,
            new
            {
                serviceCategory.Name,
                serviceCategory.Duration,
                serviceCategory.IsActive,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    public async Task UpdateAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         UPDATE "ServiceCategories" SET "Name"=@Name, 
         "Duration"=@Duration, "UpdatedOnUtc"=@UpdatedOnUtc
         WHERE "Id"=@Id;
         """,
            new
            {
                Id = (int)serviceCategory.Id,
                serviceCategory.Name,
                serviceCategory.Duration,
                UpdatedOnUtc = DateTime.Now
            });
    }

    public async Task DeleteAsync(ServiceCategory serviceCategory, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
            ("""Delete from "ServiceCategories" where "Id" = @Id""", new { Id = (int)serviceCategory.Id });
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "ServiceCategories" where "Name" = @name limit 1""",
            new { name });
        return result is not null;
    }
}