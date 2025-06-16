using Clinic.Domain.Abstractions;

namespace Clinic.Application.Abstractions;

public interface IOtpService
{
    public Task<Result<string>> GenerateOtpAsync(string key, CancellationToken cancellationToken = default);

    public Task<Result> CheckOtpAsync(string key, string otp, CancellationToken cancellationToken = default);
}