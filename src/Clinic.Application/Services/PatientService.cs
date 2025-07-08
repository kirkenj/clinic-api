using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.Patients;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Filters;
using Clinic.Domain.Repositories;

namespace Clinic.Application.Services;
public class PatientService
    (IPatientRepository patientRepository,
    IOtpService otpService,
    IEmailService emailService,
    IJwtService jwtService)
    : IPatientService
{
    public async Task<Result<string>> ConfirmSignIn(string email, string otp, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        otp = otp.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(otp);
        var otpKey = $"PatientSignInRequest:{email}";
        var otpGenerationResult = await otpService.CheckOtpAsync(otpKey, otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var patient = await patientRepository.GetByEmailAsync(email, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<string>(PatientErrors.NotFound(email));
        }

        if (!patient.IsActive)
        {
            return Result.Failure<string>(PatientErrors.NotActive(patient.Id));
        }

        return Result.Success(jwtService.GenerateToken(patient.Id, RoleType.Patient));
    }

    public async Task<Result> ConfirmSignUp(string email, string otp, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        otp = otp.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(otp);
        var otpKey = $"AddPatientCommand:{email}";
        var otpGenerationResult = await otpService.CheckOtpAsync(otpKey, otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return otpGenerationResult.Error;
        }

        var patient = await patientRepository.GetByEmailAsync(email, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(email);
        }

        if (patient.IsActive)
        {
            return PatientErrors.Active(patient.Id);
        }

        patient.Activate();
        await patientRepository.UpdateAsync(patient, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(uint id, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(id);
        }

        await patientRepository.DeleteAsync(patient, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<PatientResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<PatientResponse>(
                PatientErrors.NotFound(id));
        }

        return Result.Success(PatientResponse.FromPatient(patient));
    }

    public async Task<Result<PaginatedCollection<PatientResponse>>> GetPaginatedAsync(
        PatientFilter? patientFilter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var paginatedPatients = await patientRepository.GetPaginatedAsync(
            patientFilter, page, pageSize, cancellationToken);
        var patientResponse = paginatedPatients.Select(PatientResponse.FromPatient);
        return Result.Success(patientResponse);
    }

    public async Task<Result> ResendSignUpOtp(string email, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        var patient = await patientRepository.GetByEmailAsync(email, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(email);
        }

        if (patient.IsActive)
        {
            return PatientErrors.Active(patient.Id);
        }

        var otpKey = $"AddPatientCommand:{email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return otpGenerationResult.Error;
        }

        await emailService.SendAsync(patient.Email, Constants.SubjectSignUp,
            EmailBodyBuilder.BuildSignUpContent(otpGenerationResult.Value), cancellationToken);
        return Result.Success();
    }

    public async Task<Result<string>> SignIn(string email, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email, nameof(email));

        var patient = await patientRepository.GetByEmailAsync(email, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<string>(PatientErrors.NotFound(email));
        }

        if (!patient.IsActive)
        {
            return Result.Failure<string>(PatientErrors.NotActive(patient.Id));
        }

        var otpKey = $"PatientSignInRequest:{patient.Email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        await emailService.SendAsync(patient.Email, Constants.SubjectSignIn,
            EmailBodyBuilder.BuildSignInContent(otpGenerationResult.Value), cancellationToken);

        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);
    }

    public async Task<Result<string>> SignUp(string firstName, string middleName, string lastName,
        string email, string phoneNumber, CancellationToken cancellationToken)
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
        var isPhoneTaken = await patientRepository.ExistsByPhoneNumberAsync(phoneNumber, cancellationToken);
        if (isPhoneTaken)
        {
            return Result.Failure<string>(PatientErrors.PhoneAlreadyTaken(phoneNumber));
        }

        var isEmailTaken = await patientRepository.ExistsByEmailAsync(email, cancellationToken);
        if (isEmailTaken)
        {
            return Result.Failure<string>(PatientErrors.EmailAlreadyTaken(email));
        }

        var otpKey = $"AddPatientCommand:{email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var patient = Patient.Create(firstName, lastName, middleName, email, phoneNumber);
        patient.Deactivate();
        await patientRepository.AddAsync(patient, cancellationToken);
        await emailService.SendAsync(patient.Email, Constants.SubjectSignUp,
            EmailBodyBuilder.BuildSignUpContent(otpGenerationResult.Value), cancellationToken);
        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);

    }

    public async Task<Result> UpdateAsync(uint id, string lastName, string firstName, string middleName, string phoneNumber, string email, CancellationToken cancellationToken)
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
        var patient = await patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(id);
        }

        if (!patient.IsActive)
        {
            return PatientErrors.NotActive(patient.Id);
        }

        var hasSamePhone = patient.PhoneNumber.HasSameValue(phoneNumber);
        if (!hasSamePhone)
        {
            var isPhoneTaken = await patientRepository.ExistsByPhoneNumberAsync(
                phoneNumber, cancellationToken);
            if (isPhoneTaken)
            {
                return PatientErrors.PhoneAlreadyTaken(phoneNumber);
            }
        }

        var hasSameEmail = patient.Email.HasSameValue(email);
        if (!hasSameEmail)
        {
            var isEmailTaken = await patientRepository.ExistsByEmailAsync(email, cancellationToken);
            if (isEmailTaken)
            {
                return PatientErrors.EmailAlreadyTaken(email);
            }
        }

        patient.Update(firstName, lastName, middleName, email, phoneNumber);
        await patientRepository.UpdateAsync(patient, cancellationToken);
        return Result.Success();
    }
}
