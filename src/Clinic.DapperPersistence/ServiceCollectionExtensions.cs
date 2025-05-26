using Clinic.DapperPersistence.Absctractions;
using Clinic.DapperPersistence.Models;
using Clinic.DapperPersistence.Repositories;
using Clinic.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.DapperPersistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>(sp =>
        {
            var connectionString = configuration.GetConnectionString("ClinicConnection")!;
            return new SqlConnectionFactory(connectionString);
        });

        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IReceptionistRepository, ReceptionistRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();

        return services;
    }
}