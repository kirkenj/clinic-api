using Clinic.DapperPersistence.Absctractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Dapper;

namespace Clinic.DapperPersistence.Repositories;

public sealed class PatientRepository(ISqlConnectionFactory sqlConnectionFactory) : IPatientRepository
{
    public async Task<Patient?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryFirstOrDefaultAsync<Patient>
            ("""SELECT * FROM "Patients" where "Id" = @id""", new { id = (int)id });
        return result;
    }

    public async Task<ICollection<Patient>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryAsync<Patient>
            ("""SELECT * FROM "Patients";""", cancellationToken);
        return result.ToList();
    }

    public async Task<PaginatedCollection<Patient>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var resultSet = await sqlConnection.QueryAsync<Patient>
        ("""SELECT * FROM public."Patients" offset @offset limit @limit""",
            new { limit = pageSize, offset = (page - 1) * pageSize });
        var totalCount = await sqlConnection.QueryFirstAsync<int>
            ("""SELECT COUNT(*) FROM public."Patients" """);


        return new PaginatedCollection<Patient>(resultSet, page, pageSize, totalCount);
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         INSERT INTO "Patients" ("IsActive", "FirstName", "LastName", "MiddleName", 
         "PhoneNumber", "Email", "CreatedOnUtc") 
         VALUES(@IsActive, @FirstName, @LastName, @MiddleName, 
         @PhoneNumber, @Email, @CreatedOnUtc)
         """,
            new
            {
                patient.IsActive,
                patient.FirstName,
                patient.LastName,
                patient.MiddleName,
                patient.PhoneNumber,
                patient.Email,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         UPDATE "Patients" SET "FirstName"=@FirstName, "LastName"=@LastName, "MiddleName"=@MiddleName, 
         "PhoneNumber"=@PhoneNumber, "Email"=@Email, "UpdatedOnUtc"=@UpdatedOnUtc 
         WHERE "Id"=@Id;
         """,
            new
            {
                Id = (int)patient.Id,
                patient.FirstName,
                patient.LastName,
                patient.MiddleName,
                patient.PhoneNumber,
                patient.Email,
                UpdatedOnUtc = DateTime.Now
            });
    }

    public async Task DeleteAsync(Patient patient, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
            ("""Delete from "Patients" where "Id" = @Id""", new { Id = (int)patient.Id });
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Patients" where "PhoneNumber" = @PhoneNumber limit 1""",
            new { phoneNumber });
        return result is not null;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Patients" where "Email" = @email limit 1""",
            new { email });
        return result is not null;
    }
}