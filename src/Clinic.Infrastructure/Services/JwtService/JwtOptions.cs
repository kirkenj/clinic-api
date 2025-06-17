namespace Clinic.Infrastructure.Services.JwtService;

public class JwtOptions
{
    public string Key { get; init; } = null!;
    public double DurationInMinutes { get; init; }
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SecurityAlgorithm { get; init; } = null!;
}