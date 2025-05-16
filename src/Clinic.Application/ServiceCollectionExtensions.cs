using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(executingAssembly); });

        ValidatorOptions.Global.LanguageManager.Enabled = false;
        services.AddValidatorsFromAssembly(executingAssembly);

        return services;
    }
}