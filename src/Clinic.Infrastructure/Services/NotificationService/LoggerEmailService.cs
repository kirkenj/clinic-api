using Clinic.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Clinic.Infrastructure.Services.NotificationService;

public sealed class LoggerEmailService(ILogger<LoggerEmailService> logger) : IEmailService
{
    public Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken)
    {
        logger.LogWarning("Address:{email}; Subject:{subject}, Body:{body}", email, subject, body);
        return Task.CompletedTask;
    }
}