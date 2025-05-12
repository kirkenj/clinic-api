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
        if (doctor.PhoneNumber != trimmedPhone)
        {
            var isPhoneTakenByReceptionist = await receptionistRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken);
            if (isPhoneTakenByReceptionist)
            {
                return ReceptionistErrors.PhoneAlreadyTaken(trimmedPhone);
            }

            var isPhoneTakenByDoctor = await doctorRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken);
            if (isPhoneTakenByDoctor)
            {
                return DoctorErrors.PhoneAlreadyTaken(trimmedPhone);
            }
        }

        var trimmedEmail = request.Email.Trim();
        if (doctor.Email != trimmedEmail)
        {
            var isEmailTakenByReceptionist = await receptionistRepository.ExistsByEmailAsync(trimmedEmail, cancellationToken);
            if (isEmailTakenByReceptionist)
            {
                return ReceptionistErrors.EmailAlreadyTaken(trimmedEmail);
            }

            var isEmailTakenByDoctor = await doctorRepository.ExistsByEmailAsync(trimmedEmail, cancellationToken);
            if (isEmailTakenByDoctor)
            {
                return DoctorErrors.EmailAlreadyTaken(trimmedEmail);
            }
        }

        doctor.Update(request.FirstName, request.LastName, request.MiddleName,
            trimmedEmail, trimmedPhone, request.SpecializationId, request.OfficeId);

        await doctorRepository.UpdateAsync(doctor, cancellationToken);

        return Result.Success();
    }
}