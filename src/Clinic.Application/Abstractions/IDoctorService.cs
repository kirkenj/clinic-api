using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;

namespace Clinic.Application.Abstractions;
public interface IDoctorService
{
    public Task<Result> AddAsync(string firstName, string middleName, string lastName, string email, 
        string phoneNumber, uint officeId, uint specializationId, CancellationToken cancellationToken);
    public Task<Result> DeleteAsync(uint id, CancellationToken cancellationToken);
    public Task<Result<DoctorResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken);
    public Task<Result<PaginatedCollection<DoctorResponse>>> GetPaginated(DoctorFilter? doctorFilter,
        int page, int pageSize, CancellationToken cancellationToken);
    public Task<Result<string>> SendSignInOtpAsync(string email, CancellationToken cancellationToken);
    public Task<Result> UpdateAsync(uint id, string firstName, string middleName, string lastName, 
        string phoneNumber, string email, uint specializationId, uint officeId, CancellationToken cancellationToken);
    public Task<Result<string>> ValidateSignInAsync(string email, string otp, CancellationToken cancellationToken);
}
