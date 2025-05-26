using Clinic.DapperPersistence.Absctractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Dapper;

namespace Clinic.DapperPersistence.Repositories;

public sealed class OfficeRepository(ISqlConnectionFactory sqlConnectionFactory) : IOfficeRepository
{
    public async Task<Office?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryFirstOrDefaultAsync<Office>
            ("""SELECT * FROM "Offices" where "Id" = @id""", new { id = (int)id });
        return result;
    }

    public async Task<ICollection<Office>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryAsync<Office>
            ("""SELECT * FROM "Offices";""", cancellationToken);
        return result.ToList();
    }

    public async Task<PaginatedCollection<Office>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var resultSet = await sqlConnection.QueryAsync<Office>
        ("""SELECT * FROM public."Offices" offset @offset limit @limit""",
            new { limit = pageSize, offset = (page - 1) * pageSize });
        var totalCount = await sqlConnection.QueryFirstAsync<int>
            ("""SELECT COUNT(*) FROM public."Offices" """);


        return new PaginatedCollection<Office>(resultSet, page, pageSize, totalCount);
    }

    public async Task AddAsync(Office office, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         INSERT INTO "Offices" 
         ("Address", "PhoneNumber", "IsActive", "CreatedOnUtc") 
         VALUES(@Address, @PhoneNumber, false, @IsActive, @CreatedOnUtc)
         """,
            new
            {
                office.Address,
                office.PhoneNumber,
                office.IsActive,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    public async Task UpdateAsync(Office office, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         UPDATE "Offices" SET "Address"=@Address, "PhoneNumber"=@PhoneNumber, 
         "IsActive"=@IsActive, "UpdatedOnUtc"=@UpdatedOnUtc 
         WHERE "Id"=@Id;
         """,
            new
            {
                Id = (int)office.Id,
                office.Address,
                office.PhoneNumber,
                office.IsActive,
                UpdatedOnUtc = DateTime.Now
            });
    }

    public async Task DeleteAsync(Office office, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
            ("""Delete from "Offices" where "Id" = @Id""", new { Id = (int)office.Id });
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Offices" where "PhoneNumber" = @PhoneNumber limit 1""",
            new { phoneNumber });
        return result is not null;
    }
}