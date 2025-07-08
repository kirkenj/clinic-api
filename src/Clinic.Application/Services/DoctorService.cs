using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.Doctors;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Filters;
using Clinic.Domain.Repositories;

namespace Clinic.Application.Services;
public class DoctorService
    (IDoctorRepository doctorRepository,
    IReceptionistRepository receptionistRepository,
    ISpecializationRepository specializationRepository,
    IOtpService otpService,
    IEmailService emailService,
    IJwtService jwtService,
    IOfficeRepository officeRepository)
    : IDoctorService
{
    public async Task<Result> AddAsync(string firstName, string middleName, string lastName, string email,
        string phoneNumber, uint officeId, uint specializationId, CancellationToken cancellationToken)
    {
        lastName = lastName.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(lastName);
        firstName = firstName.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(firstName);
        middleName = middleName.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(middleName);
        phoneNumber = phoneNumber.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);

        var isPhoneTaken = await doctorRepository.ExistsByPhoneNumberAsync(phoneNumber, cancellationToken);
        if (isPhoneTaken)
        {
            return DoctorErrors.PhoneAlreadyTaken(phoneNumber);
        }

        var isEmailTaken = await receptionistRepository.ExistsByEmailAsync(email, cancellationToken);
        if (isEmailTaken)
        {
            return ReceptionistErrors.EmailAlreadyTaken(email);
        }

        isEmailTaken = await doctorRepository.ExistsByEmailAsync(email, cancellationToken);
        if (isEmailTaken)
        {
            return DoctorErrors.EmailAlreadyTaken(email);
        }

        var office = await officeRepository.GetByIdAsync(officeId, cancellationToken);
        if (office is null)
        {
            return OfficeErrors.NotFound(officeId);
        }

        if (!office.IsActive)
        {
            return OfficeErrors.NotActive(officeId);
        }

        var specialization = await specializationRepository.GetByIdAsync(
            specializationId, cancellationToken);
        if (specialization is null)
        {
            return SpecializationErrors.NotFound(specializationId);
        }

        if (!specialization.IsActive)
        {
            return SpecializationErrors.NotActive(specializationId);
        }

        var doctor = Doctor.Create(firstName, lastName,
            middleName, email, phoneNumber,
            specializationId, officeId);
        await doctorRepository.AddAsync(doctor, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(uint id, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor is null)
        {
            return DoctorErrors.NotFound(id);
        }

        await doctorRepository.DeleteAsync(doctor, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<DoctorResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<DoctorResponse>(
                DoctorErrors.NotFound(id));
        }

        return DoctorResponse.FromDoctor(doctor);
    }

    public async Task<Result<PaginatedCollection<DoctorResponse>>> GetPaginated(DoctorFilter? doctorFilter,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var paginatedDoctors = await doctorRepository.GetPaginatedAsync(doctorFilter,
            page, pageSize, cancellationToken);
        var doctorResponse = paginatedDoctors.Select(DoctorResponse.FromDoctor);
        return Result.Success(doctorResponse);
    }

    public async Task<Result<string>> SendSignInOtpAsync(string email, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        var doctor = await doctorRepository.GetByEmailAsync(email, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(email));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<string>(DoctorErrors.NotActive(doctor.Id));
        }

        var otpKey = $"DoctorSignInRequest:{doctor.Email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        await emailService.SendAsync(doctor.Email, Constants.SubjectSignIn,
            EmailBodyBuilder.BuildSignInContent(otpGenerationResult.Value), cancellationToken);

        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);
    }

    public async Task<Result> UpdateAsync(uint id, string firstName, string middleName, string lastName,
        string phoneNumber, string email, uint specializationId, uint officeId, CancellationToken cancellationToken)
    {
        lastName = lastName.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(lastName);
        firstName = firstName.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(firstName);
        middleName = middleName.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(middleName);
        phoneNumber = phoneNumber.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);

        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor is null)
        {
            return DoctorErrors.NotFound(id);
        }

        if (!doctor.IsActive)
        {
            return DoctorErrors.NotActive(doctor.Id);
        }

        if (officeId != doctor.OfficeId)
        {
            var office = await officeRepository.GetByIdAsync(officeId, cancellationToken);
            if (office is null)
            {
                return OfficeErrors.NotFound(officeId);
            }

            if (!office.IsActive)
            {
                return OfficeErrors.NotActive(officeId);
            }
        }

        if (specializationId != doctor.SpecializationId)
        {
            var specialization =
                await specializationRepository.GetByIdAsync(specializationId, cancellationToken);
            if (specialization is null)
            {
                return SpecializationErrors.NotFound(specializationId);
            }

            if (!specialization.IsActive)
            {
                return SpecializationErrors.NotActive(specializationId);
            }
        }

        var hasSamePhone = doctor.PhoneNumber.HasSameValue(phoneNumber);
        if (!hasSamePhone)
        {
            var isPhoneTaken =
                await receptionistRepository.ExistsByPhoneNumberAsync(phoneNumber, cancellationToken);
            if (isPhoneTaken)
            {
                return ReceptionistErrors.PhoneAlreadyTaken(phoneNumber);
            }

            isPhoneTaken = await doctorRepository.ExistsByPhoneNumberAsync(phoneNumber, cancellationToken);
            if (isPhoneTaken)
            {
                return DoctorErrors.PhoneAlreadyTaken(phoneNumber);
            }
        }

        var hasSameEmail = doctor.Email.HasSameValue(email);
        if (!hasSameEmail)
        {
            var isEmailTaken = await receptionistRepository.ExistsByEmailAsync(email, cancellationToken);
            if (isEmailTaken)
            {
                return ReceptionistErrors.EmailAlreadyTaken(email);
            }

            isEmailTaken = await doctorRepository.ExistsByEmailAsync(email, cancellationToken);
            if (isEmailTaken)
            {
                return DoctorErrors.EmailAlreadyTaken(email);
            }
        }

        doctor.Update(firstName, lastName, middleName, email, phoneNumber,
            specializationId, officeId);
        await doctorRepository.UpdateAsync(doctor, cancellationToken);
        return Result.Success();

    }

    public async Task<Result<string>> ValidateSignInAsync(string email, string otp, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        otp = otp.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(otp);
        var otpKey = $"DoctorSignInRequest:{email}";
        var otpGenerationResult = await otpService.CheckOtpAsync(otpKey, otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var doctor = await doctorRepository.GetByEmailAsync(email, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(email));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<string>(DoctorErrors.NotActive(doctor.Id));
        }

        return Result.Success(jwtService.GenerateToken(doctor.Id, RoleType.Doctor));
    }
}
