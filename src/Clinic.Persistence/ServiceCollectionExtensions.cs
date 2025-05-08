using Clinic.Domain.Office;
using Clinic.Domain.Speciallization;
using Clinic.Persistence.Interceptors;
using Clinic.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClinnicContext>((sp, options) =>
        {
            var auditableEntitiesInterceptor = sp.GetRequiredService<AuditableEntitiesInterceptor>();


            options.UseNpgsql(configuration.GetConnectionString("ClinicConnetion"));
            options.AddInterceptors(auditableEntitiesInterceptor);
        });

        services.AddSingleton<AuditableEntitiesInterceptor>();

        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<ISpeciallizationRepository, SpeciallizationRepository>();

        return services;
    }
}