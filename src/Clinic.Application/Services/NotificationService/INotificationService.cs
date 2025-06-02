using Clinic.Domain.Entities;

namespace Clinic.Application.Services.NotificationService;

public interface INotificationService
{
    public Task Notify(Patient patient, string subject, string body);
    public Task Notify(Receptionist receptionist, string subject, string body);
    public Task Notify(Doctor doctor, string subject, string body);
}