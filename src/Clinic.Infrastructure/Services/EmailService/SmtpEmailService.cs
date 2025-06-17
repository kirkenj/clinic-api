using Clinic.Application.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Clinic.Infrastructure.Services.EmailService;

public sealed class SmtpEmailService(IOptions<SmtpEmailServiceOptions> options) : IEmailService
{
    public async Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken)
    {
        using var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(options.Value.FromName, options.Value.ApiLogin));
        emailMessage.To.Add(new MailboxAddress("User", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Value.ApiAddress, options.Value.ApiPort, true, cancellationToken);
        await client.AuthenticateAsync(options.Value.ApiLogin, options.Value.ApiPassword, cancellationToken);
        await client.SendAsync(emailMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}