using Clinic.Domain.Abstractions;
using Clinic.Domain.Doctor;
using Clinic.Domain.Office;
using Clinic.Domain.Specialization;
using MediatR;

namespace Clinic.Application.Features.Doctor.Commands.Update;
public sealed class UpdateDoctorCommandHandler(
    IDoctorRepository DoctorRepository,
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

        var doctor = await DoctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null)
        {
            return DoctorErrors.NotFound(request.Id);
        }

        if (!doctor.IsActive)
        {
            return DoctorErrors.NotActive(doctor.Id);
        }

        var trimmedPhone = request.PhoneNumber.Trim();
        if (doctor.PhoneNumber != trimmedPhone)
        {
            var isPhoneTaken = await DoctorRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken);
            if (isPhoneTaken)
            {
                return DoctorErrors.PhoneAlreadyTaken(trimmedPhone);
            }
        }

        var trimmedEmail = request.Email.Trim();
        if (doctor.Email != trimmedEmail)
        {
            var isEmailTaken = await DoctorRepository.ExistsByEmailAsync(trimmedEmail, cancellationToken);
            if (isEmailTaken)
            {
                return DoctorErrors.EmailAlreadyTaken(trimmedEmail);
            }
        }

        if (request.OfficeId != doctor.OfficeId)
        {
            var office = await officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
            if (office == null)
            {
                return OfficeErrors.NotFound(request.OfficeId);
            }
            else if (!office.IsActive)
            {
                return OfficeErrors.NotActive(request.OfficeId);
            }
        }

        if (request.SpecializationId != doctor.SpecializationId)
        {
            var specialization = await specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
            if (specialization == null)
            {
                return SpecializationErrors.NotFound(request.SpecializationId);
            }
            else if (!specialization.IsActive)
            {
                return SpecializationErrors.NotActive(request.SpecializationId);
            }
        }

        doctor.Update(request.FirstName, request.LastName, request.MiddleName,
            trimmedEmail, trimmedPhone, request.SpecializationId, request.OfficeId);

        await DoctorRepository.UpdateAsync(doctor, cancellationToken);

        return Result.Success();
    }
}