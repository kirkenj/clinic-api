using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Update;
public sealed class UpdateDoctorCommandHandler(
    IReceptionistRepository receptionistRepository,
    IDoctorRepository doctorRepository,
    ISpecializationRepository specializationRepository,
    IOfficeRepository officeRepository,
    UpdateDoctorCommandValidator validator
    ) : IRequestHandler<UpdateDoctorCommand, Result>
{
    public async Task<Result> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var doctor = await doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor is null)
        {
            return DoctorErrors.NotFound(request.Id);
        }

        if (!doctor.IsActive)
        {
            return DoctorErrors.NotActive(doctor.Id);
        }

        if (request.OfficeId != doctor.OfficeId)
        {
            var office = await officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
            if (office is null)
            {
                return OfficeErrors.NotFound(request.OfficeId);
            }

            if (!office.IsActive)
            {
                return OfficeErrors.NotActive(request.OfficeId);
            }
        }

        if (request.SpecializationId != doctor.SpecializationId)
        {
            var specialization = await specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
            if (specialization is null)
            {
                return SpecializationErrors.NotFound(request.SpecializationId);
            }

            if (!specialization.IsActive)
            {
                return SpecializationErrors.NotActive(request.SpecializationId);
            }
        }

        var trimmedPhone = request.PhoneNumber.Trim();
        var hasSamePhone = string.Equals(doctor.PhoneNumber, trimmedPhone, StringComparison.InvariantCultureIgnoreCase);
        if (!hasSamePhone) 
        {
            var isPhoneTaken = await receptionistRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken);
            if (isPhoneTaken)
            {
                return ReceptionistErrors.PhoneAlreadyTaken(trimmedPhone);
            }

            isPhoneTaken = await doctorRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken);
            if (isPhoneTaken)
            {
                return DoctorErrors.PhoneAlreadyTaken(trimmedPhone);
            }
        }

        var trimmedEmail = request.Email.Trim();
        var hasSameEmail = string.Equals(doctor.Email, trimmedEmail, StringComparison.InvariantCultureIgnoreCase);
        if (!hasSameEmail)
        {
            var isEmailTaken = await receptionistRepository.ExistsByEmailAsync(trimmedEmail, cancellationToken);
            if (isEmailTaken)
            {
                return ReceptionistErrors.EmailAlreadyTaken(trimmedEmail);
            }

            isEmailTaken = await doctorRepository.ExistsByEmailAsync(trimmedEmail, cancellationToken);
            if (isEmailTaken)
            {
                return DoctorErrors.EmailAlreadyTaken(trimmedEmail);
            }
        }

        var trimmedFirstName = request.FirstName.Trim();
        var trimmedLastName = request.LastName.Trim();
        var trimmedMiddleName = request.MiddleName.Trim();
        doctor.Update(trimmedFirstName, trimmedLastName, 
            trimmedMiddleName, trimmedEmail, trimmedPhone, 
            request.SpecializationId, request.OfficeId);

        await doctorRepository.UpdateAsync(doctor, cancellationToken);

        return Result.Success();
    }
}