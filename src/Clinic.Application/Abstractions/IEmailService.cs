namespace Clinic.Application.Abstractions;

public interface IEmailService
{
    public Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken);
}