using Clinic.Application.Extensions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Update;

public sealed class UpdateDoctorCommandHandler
    (IReceptionistRepository receptionistRepository,
        IDoctorRepository doctorRepository,
        ISpecializationRepository specializationRepository,
        IOfficeRepository officeRepository,
        UpdateDoctorCommandValidator validator)
    : IRequestHandler<UpdateDoctorCommand, Result>
{
    public async Task<Result> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
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
            var specialization =
                await specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
            if (specialization is null)
            {
                return SpecializationErrors.NotFound(request.SpecializationId);
            }

            if (!specialization.IsActive)
            {
                return SpecializationErrors.NotActive(request.SpecializationId);
            }
        }

        var hasSamePhone = doctor.PhoneNumber.HasSameValue(request.PhoneNumber);
        if (!hasSamePhone)
        {
            var isPhoneTaken =
                await receptionistRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (isPhoneTaken)
            {
                return ReceptionistErrors.PhoneAlreadyTaken(request.PhoneNumber);
            }

            isPhoneTaken = await doctorRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (isPhoneTaken)
            {
                return DoctorErrors.PhoneAlreadyTaken(request.PhoneNumber);
            }
        }

        var hasSameEmail = doctor.Email.HasSameValue(request.Email);
        if (!hasSameEmail)
        {
            var isEmailTaken = await receptionistRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (isEmailTaken)
            {
                return ReceptionistErrors.EmailAlreadyTaken(request.Email);
            }

            isEmailTaken = await doctorRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (isEmailTaken)
            {
                return DoctorErrors.EmailAlreadyTaken(request.Email);
            }
        }

        doctor.Update(request.FirstName, request.LastName,
            request.MiddleName, request.Email, request.PhoneNumber,
            request.SpecializationId, request.OfficeId);
        await doctorRepository.UpdateAsync(doctor, cancellationToken);
        return Result.Success();
    }
}