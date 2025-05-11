using Clinic.Domain.Abstractions;
using Clinic.Domain.Doctor;
using Clinic.Domain.Office;
using Clinic.Domain.Specialization;
using MediatR;

namespace Clinic.Application.Features.Doctor.Commands.Add;
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
        if (office == null)
        {
            return OfficeErrors.NotFound(request.OfficeId);
        }
        else if (!office.IsActive)
        {
            return OfficeErrors.NotActive(request.OfficeId);
        }
        
        var specialization = await specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization == null)
        {
            return SpecializationErrors.NotFound(request.SpecializationId);
        }
        else if (!specialization.IsActive)
        {
            return SpecializationErrors.NotActive(request.SpecializationId);
        }


        var doctor = Domain.Doctor.Doctor.Create(request.FirstName, request.LastName, request.MiddleName,
            trimmedEmail, trimmedPhone, request.SpecializationId, request.OfficeId);

        await doctorRepository.AddAsync(doctor, cancellationToken);

        return Result.Success();
    }
}