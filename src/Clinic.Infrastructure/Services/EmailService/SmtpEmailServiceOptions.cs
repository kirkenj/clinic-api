namespace Clinic.Infrastructure.Services.EmailService;

public sealed class SmtpEmailServiceOptions
{
    public string FromName { get; init; } = null!;
    public string ApiAddress { get; init; } = null!;
    public int ApiPort { get; init; }
    public string ApiLogin { get; init; } = null!;
    public string ApiPassword { get; init; } = null!;
}