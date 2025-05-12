using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Add;
public sealed class AddDoctorCommandHandler(
    IDoctorRepository doctorRepository,
    ISpecializationRepository specializationRepository,
    IOfficeRepository officeRepository,
    AddDoctorCommandValidator validator
    ) : IRequestHandler<AddDoctorCommand, Result>
{
    public async Task<Result> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var trimmedPhone = request.PhoneNumber.Trim();
        var isPhoneTaken = await doctorRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken);
        if (isPhoneTaken)
        {
            return DoctorErrors.PhoneAlreadyTaken(trimmedPhone);
        }

        var trimmedEmail = request.Email.Trim();
        var isEmailTaken = await doctorRepository.ExistsByEmailAsync(trimmedEmail, cancellationToken);
        if (isEmailTaken)
        {
            return DoctorErrors.EmailAlreadyTaken(trimmedEmail);
        }

        var office = await officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office is null)
        {
            return OfficeErrors.NotFound(request.OfficeId);
        }

        if (!office.IsActive)
        {
            return OfficeErrors.NotActive(request.OfficeId);
        }

        var specialization = await specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization is null)
        {
            return SpecializationErrors.NotFound(request.SpecializationId);
        }

        if (!specialization.IsActive)
        {
            return SpecializationErrors.NotActive(request.SpecializationId);
        }


        var doctor = Doctor.Create(request.FirstName, request.LastName, request.MiddleName,
            trimmedEmail, trimmedPhone, request.SpecializationId, request.OfficeId);

        await doctorRepository.AddAsync(doctor, cancellationToken);

        return Result.Success();
    }
}