using Clinic.DapperPersistence.Absctractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Dapper;

namespace Clinic.DapperPersistence.Repositories;

public class ReceptionistRepository(ISqlConnectionFactory sqlConnectionFactory) : IReceptionistRepository
{
    public async Task<Receptionist?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryFirstOrDefaultAsync<Receptionist>
            ("""SELECT * FROM "Receptionists" where "Id" = @id""", new { id = (int)id });
        return result;
    }

    public async Task<ICollection<Receptionist>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryAsync<Receptionist>
            ("""SELECT * FROM "Receptionists";""", cancellationToken);
        return result.ToList();
    }

    public async Task<PaginatedCollection<Receptionist>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var resultSet = await sqlConnection.QueryAsync<Receptionist>
        ("""SELECT * FROM public."Receptionists" offset @offset limit @limit""",
            new { limit = pageSize, offset = (page - 1) * pageSize });
        var totalCount = await sqlConnection.QueryFirstAsync<int>
            ("""SELECT COUNT(*) FROM public."Receptionists" """);


        return new PaginatedCollection<Receptionist>(resultSet, page, pageSize, totalCount);
    }

    public async Task UpdateAsync(Receptionist receptionist, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         UPDATE "Receptionists" SET "FirstName"=@FirstName, "LastName"=@LastName, "MiddleName"=@MiddleName, 
         "PhoneNumber"=@PhoneNumber, "Email"=@Email, "UpdatedOnUtc"=@UpdatedOnUtc
         WHERE "Id"=@Id;
         """,
            new
            {
                Id = (int)receptionist.Id,
                receptionist.FirstName,
                receptionist.LastName,
                receptionist.MiddleName,
                receptionist.PhoneNumber,
                receptionist.Email,
                UpdatedOnUtc = DateTime.Now
            });
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Receptionists" where "PhoneNumber" = @PhoneNumber limit 1""",
            new { phoneNumber });
        return result is not null;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Receptionists" where "Email" = @email limit 1""",
            new { email });
        return result is not null;
    }

    public async Task AddAsync(Receptionist receptionist, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         INSERT INTO "Receptionists" ("IsActive", "FirstName", "LastName", "MiddleName", 
         "PhoneNumber", "Email", "CreatedOnUtc") 
         VALUES(@IsActive, @FirstName, @LastName, @MiddleName, 
         @PhoneNumber, @Email, @CreatedOnUtc)
         """,
            new
            {
                receptionist.IsActive,
                receptionist.FirstName,
                receptionist.LastName,
                receptionist.MiddleName,
                receptionist.PhoneNumber,
                receptionist.Email,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    public async Task DeleteAsync(Receptionist receptionist, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
            ("""Delete from "Receptionists" where "Id" = @Id""", new { Id = (int)receptionist.Id });
    }
}