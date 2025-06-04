using Clinic.Application.Abstractions;
using Clinic.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Clinic.Infrastructure.Services.NotificationService;
public sealed class LoggerNotificationService(ILogger<LoggerNotificationService> logger) : INotificationService
{
    public Task NotifyAsync(Patient patient, string subject, string body, CancellationToken cancellationToken)
    {
        var msg = $"Patient:{patient.Email}; Subject:{subject}, Body:{body}";
        logger.LogWarning(msg);
        return Task.CompletedTask;
    }

    public Task NotifyAsync(Receptionist receptionist, string subject, string body, CancellationToken cancellationToken)
    {
        var msg = $"Receptionist:{receptionist.Email}; Subject:{subject}, Body:{body}";
        logger.LogWarning(msg);
        return Task.CompletedTask;
    }

    public Task NotifyAsync(Doctor doctor, string subject, string body, CancellationToken cancellationToken)
    {
        var msg = $"Doctor:{doctor.Email}; Subject:{subject}, Body:{body}";
        logger.LogWarning(msg);
        return Task.CompletedTask;
    }
}
