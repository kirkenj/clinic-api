using Clinic.Domain.Repositories;
using Clinic.Persistence.Interceptors;
using Clinic.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Persistence;

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
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IOtpRecordRepository, OtpRecordRepository>();
        services.AddScoped<IAppointmentResultRepository, AppointmentResultRepository>();

        return services;
    }
}