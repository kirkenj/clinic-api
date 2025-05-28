using System.Reflection;
using Clinic.Application.Options;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(executingAssembly); });

        var serviceOptionsConfiguration = configuration.GetSection("ServiceOptions");
        services.Configure<ServiceOptions>(serviceOptionsConfiguration);

        var appointmentsOptionsConfiguration = configuration.GetSection("AppointmentsOptions");
        services.Configure<AppointmentsOptions>(appointmentsOptionsConfiguration);

        ValidatorOptions.Global.LanguageManager.Enabled = false;
        services.AddValidatorsFromAssembly(executingAssembly);

        return services;
    }
}