using Clinic.DapperPersistence.Absctractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Dapper;

namespace Clinic.DapperPersistence.Repositories;

public sealed class ServiceRepository(ISqlConnectionFactory sqlConnectionFactory) : IServiceRepository
{
    public async Task<Service?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryFirstOrDefaultAsync<Service>
            ("""SELECT * FROM "Services" where "Id" = @id""", new { id = (int)id });
        return result;
    }

    public async Task<ICollection<Service>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryAsync<Service>
            ("""SELECT * FROM "Services";""", cancellationToken);
        return result.ToList();
    }

    public async Task<PaginatedCollection<Service>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var resultSet = await sqlConnection.QueryAsync<Service>
        ("""SELECT * FROM public."Services" offset @offset limit @limit""",
            new { limit = pageSize, offset = (page - 1) * pageSize });
        var totalCount = await sqlConnection.QueryFirstAsync<int>
            ("""SELECT COUNT(*) FROM public."Services" """);


        return new PaginatedCollection<Service>(resultSet, page, pageSize, totalCount);
    }

    public async Task AddAsync(Service service, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         INSERT INTO "Services" ("IsActive", "Name", "Price", 
         "SpecializationId", "ServiceCategoryId", "CreatedOnUtc") 
         VALUES(@IsActive, @Name, @Price, @SpecializationId, 
         @ServiceCategoryId, @CreatedOnUtc);
         """,
            new
            {
                service.IsActive,
                service.Name,
                service.Price,
                service.SpecializationId,
                service.ServiceCategoryId,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    public async Task UpdateAsync(Service service, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         UPDATE "Services" SET "Name"=@Name, "Price"=@Price, "SpecializationId"=@SpecializationId, 
         "ServiceCategoryId"=@ServiceCategoryId, "UpdatedOnUtc"=@UpdatedOnUtc
         WHERE "Id"=@Id;
         """,
            new
            {
                Id = (int)service.Id,
                service.Name,
                service.Price,
                service.SpecializationId,
                service.ServiceCategoryId,
                UpdatedOnUtc = DateTime.Now
            });
    }

    public async Task DeleteAsync(Service service, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
            ("""Delete from "Services" where "Id" = @Id""", new { Id = (int)service.Id });
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Services" where "Name" = @name limit 1""",
            new { name });
        return result is not null;
    }
}