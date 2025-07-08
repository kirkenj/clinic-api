using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;

namespace Clinic.Application.Abstractions;
public interface IReceptionistService
{
    public Task<Result<ReceptionistResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<Result<string>> SendSignInOtpAsync(string email, CancellationToken cancellationToken);
    public Task<Result<string>> ValidateSignInAsync(string email, string otp, CancellationToken cancellationToken);
}
