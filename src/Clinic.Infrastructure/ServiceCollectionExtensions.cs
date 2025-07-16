using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Infrastructure.Jobs;
using Clinic.Infrastructure.Services.DateTimeProvider;
using Clinic.Infrastructure.Services.EmailService;
using Clinic.Infrastructure.Services.HttpUserContextService;
using Clinic.Infrastructure.Services.JwtService;
using Clinic.Infrastructure.Services.OtpService;
using Clinic.Infrastructure.Services.SlotService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Text;

namespace Clinic.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<ISlotService, SlotService>();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.Configure<SmtpEmailServiceOptions>(configuration.GetSection("SmtpEmailServiceOptions"));
        services.AddTransient<IEmailService, SmtpEmailService>();
        services.Configure<OtpServiceOptions>(configuration.GetSection("OtpServiceOptions"));
        services.AddScoped<IOtpService, OtpService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserAccessService, UserAccessService>();
        var jwtOptionsSections = configuration.GetSection("JwtOptions");
        services.Configure<JwtOptions>(jwtOptionsSections);
        services.AddTransient<IJwtService, JwtService>();
        var settings = jwtOptionsSections.Get<JwtOptions>()
                       ?? throw new Exception("Couldn't get JwtOptions from appSettings");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidAlgorithms = [settings.SecurityAlgorithm]
                };
            });

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("CancelUnconfirmedAppointmentsJob");
            q.AddJob<CancelUnconfirmedAppointmentsJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("CancelUnconfirmedAppointments-Trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(30)
                    .RepeatForever()) 
                .StartNow() 
            );
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}