using Clinic.DapperPersistence.Interceptors;
using Clinic.DapperPersistence.Repositories;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.DapperPersistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ClinnicContext>((sp, options) =>
        {
            var auditableEntitiesInterceptor = sp.GetRequiredService<AuditableEntitiesInterceptor>();

            options.UseNpgsql(configuration.GetConnectionString("ClinicConnection"));
            options.AddInterceptors(auditableEntitiesInterceptor);
        });

        services.AddSingleton<AuditableEntitiesInterceptor>();

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