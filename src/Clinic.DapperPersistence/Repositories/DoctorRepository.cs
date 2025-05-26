using Clinic.DapperPersistence.Absctractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Dapper;

namespace Clinic.DapperPersistence.Repositories;

public sealed class DoctorRepository(ISqlConnectionFactory sqlConnectionFactory) : IDoctorRepository
{
    public async Task<Doctor?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryFirstOrDefaultAsync<Doctor>
            ("""SELECT * FROM "Doctors" where "Id" = @id""", new { id = (int)id });
        return result;
    }

    public async Task<ICollection<Doctor>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QueryAsync<Doctor>
            ("""SELECT * FROM "Doctors";""", cancellationToken);
        return result.ToList();
    }

    public async Task<PaginatedCollection<Doctor>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var resultSet = await sqlConnection.QueryAsync<Doctor>
        ("""SELECT * FROM public."Doctors" offset @offset limit @limit""",
            new { limit = pageSize, offset = (page - 1) * pageSize });
        var totalCount = await sqlConnection.QueryFirstAsync<int>
            ("""SELECT COUNT(*) FROM public."Doctors" """);


        return new PaginatedCollection<Doctor>(resultSet, page, pageSize, totalCount);
    }

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         INSERT INTO "Doctors" ("IsActive", "FirstName", "LastName", "MiddleName", 
         "PhoneNumber", "Email", "SpecializationId", "OfficeId", "CreatedOnUtc") 
         VALUES(@IsActive, @FirstName, @LastName, @MiddleName, 
         @PhoneNumber, @Email, @SpecializationId, @OfficeId, @CreatedOnUtc)
         """,
            new
            {
                doctor.IsActive,
                doctor.FirstName,
                doctor.LastName,
                doctor.MiddleName,
                doctor.PhoneNumber,
                doctor.Email,
                doctor.SpecializationId,
                doctor.OfficeId,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
        ("""
         UPDATE "Doctors" SET "FirstName"=@FirstName, "LastName"=@LastName, "MiddleName"=@MiddleName, 
         "PhoneNumber"=@PhoneNumber, "Email"=@Email, "SpecializationId"=@SpecializationId, 
         "OfficeId"=@OfficeId, "UpdatedOnUtc"=@UpdatedOnUtc 
         WHERE "Id"=@Id;
         """,
            new
            {
                Id = (int)doctor.Id,
                doctor.FirstName,
                doctor.LastName,
                doctor.MiddleName,
                doctor.PhoneNumber,
                doctor.Email,
                doctor.SpecializationId,
                doctor.OfficeId,
                UpdatedOnUtc = DateTime.Now
            });
    }

    public async Task DeleteAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        await sqlConnection.ExecuteAsync
            ("""Delete from "Doctors" where "Id" = @Id""", new { Id = (int)doctor.Id });
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Doctors" where "PhoneNumber" = @PhoneNumber limit 1""",
            new { phoneNumber });
        return result is not null;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using var sqlConnection = sqlConnectionFactory.CreateConnection();
        var result = await sqlConnection.QuerySingleOrDefaultAsync<int?>
        ("""select "Id" FROM "Doctors" where "Email" = @email limit 1""",
            new { email });
        return result is not null;
    }
}