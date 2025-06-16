using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Infrastructure.Services.DateTimeService;
using Clinic.Infrastructure.Services.HashService;
using Clinic.Infrastructure.Services.NotificationService;
using Clinic.Infrastructure.Services.OtpService;
using Clinic.Infrastructure.Services.SlotService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IHashService, HashService>();
        services.AddTransient<ISlotService, SlotService>();
        services.AddTransient<IEmailService, LoggerEmailService>();
        services.AddTransient<IDateTimeService, DateTimeService>();

        services.Configure<OtpServiceOptions>(configuration.GetSection("OtpServiceOptions"));
        services.AddScoped<IOtpService, OtpService>();

        return services;
    }
}