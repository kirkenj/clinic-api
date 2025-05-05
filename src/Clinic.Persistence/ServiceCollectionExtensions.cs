using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Clinic.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClinnicContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ClinicConnetion"));
        });
        return services;
    }
}