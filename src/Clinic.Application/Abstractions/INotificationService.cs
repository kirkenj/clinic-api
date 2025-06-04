using Clinic.Domain.Entities;

namespace Clinic.Application.Abstractions;

public interface INotificationService
{
    public Task NotifyAsync(Patient patient, string subject, string body, CancellationToken cancellationToken);
    public Task NotifyAsync(Receptionist receptionist, string subject, string body, CancellationToken cancellationToken);
    public Task NotifyAsync(Doctor doctor, string subject, string body, CancellationToken cancellationToken);
}