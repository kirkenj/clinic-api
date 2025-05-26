using System.Data;

namespace Clinic.DapperPersistence.Absctractions;

public interface ISqlConnectionFactory
{
    public IDbConnection CreateConnection();
}