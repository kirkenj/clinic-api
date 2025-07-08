using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;

namespace Clinic.Application.Abstractions;
public interface IPatientService
{
    public Task<Result<string>> ConfirmSignIn(string email, string otp, CancellationToken cancellationToken);
    public Task<Result> ConfirmSignUp(string email, string otp, CancellationToken cancellationToken);
    public Task<Result> DeleteAsync(uint id, CancellationToken cancellationToken);
    public Task<Result<PatientResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<Result<PaginatedCollection<PatientResponse>>> GetPaginatedAsync(
        PatientFilter? patientFilter, int page, int pageSize, CancellationToken cancellationToken);
    public Task<Result> ResendSignUpOtp(string email, CancellationToken cancellationToken);
    public Task<Result<string>> SignIn(string email, CancellationToken cancellationToken);
    public Task<Result<string>> SignUp(string firstName, string middleName, 
        string lastName, string email, string phoneNumber, CancellationToken cancellationToken);
    public Task<Result> UpdateAsync(uint id, string lastName, string firstName, 
        string middleName, string phoneNumber, string email, CancellationToken cancellationToken);
}
