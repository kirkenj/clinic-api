using System.Data;
using Clinic.DapperPersistence.Absctractions;
using Npgsql;

namespace Clinic.DapperPersistence.Models;

public sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}