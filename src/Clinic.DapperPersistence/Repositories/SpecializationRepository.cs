using Clinic.DapperPersistence.Absctractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Dapper;

namespace Clinic.DapperPersistence.Repositories;

public class SpecializationRepository(ISqlConnectionFactory sqlConnectionFactory) : ISpecializationRepository
{
    public async Task<Specialization?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryFirstOrDefaultAsync<Specialization>
            ("""SELECT * FROM "Specializations" where "Id" = @id""", new { id = (int)id });
        return result;
    }

    public async Task<ICollection<Specialization>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryAsync<Specialization>
            ("""SELECT * FROM "Specializations";""", cancellationToken);
        return result.ToList();
    }

    public async Task<PaginatedCollection<Specialization>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var resultSet = await sqlConnection.QueryAsync<Specialization>
        ("""SELECT * FROM public."Specializations" offset @offset limit @limit""",
            new { limit = pageSize, offset = (page - 1) * pageSize });
        var totalCount = await sqlConnection.QueryFirstAsync<int>
            ("""SELECT COUNT(*) FROM public."Specializations" """);


        return new PaginatedCollection<Specialization>(resultSet, page, pageSize, totalCount);
    }

    public async Task AddAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         INSERT INTO "Specializations" ("IsActive", "Name", "CreatedOnUtc") 
         VALUES(@IsActive, @Name, @CreatedOnUtc);
         """,
            new
            {
                specialization.IsActive,
                specialization.Name,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    public async Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         UPDATE "Specializations" SET "Name"=@Name, "UpdatedOnUtc"=@UpdatedOnUtc
         WHERE "Id"=@Id;
         """,
            new
            {
                Id = (int)specialization.Id,
                specialization.Name,
                UpdatedOnUtc = DateTime.Now
            });
    }

    public async Task DeleteAsync(Specialization Specialization, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
            ("""Delete from "Specializations" where "Id" = @Id""", new { Id = (int)Specialization.Id });
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Specializations" where "Name" = @name limit 1""",
            new { name });
        return result is not null;
    }
}