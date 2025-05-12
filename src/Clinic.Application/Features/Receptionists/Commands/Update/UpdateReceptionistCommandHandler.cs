using System.Numerics;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.Update;
public sealed class UpdateReceptionistCommandHandler(
    IReceptionistRepository receptionistRepository,
    IDoctorRepository doctorRepository,
    UpdateReceptionistCommandValidator validator
    ) : IRequestHandler<UpdateReceptionistCommand, Result>
{
    public async Task<Result> Handle(UpdateReceptionistCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var receptionist = await receptionistRepository.GetByIdAsync(request.Id, cancellationToken);
        if (receptionist is null)
        {
            return ReceptionistErrors.NotFound(request.Id);
        }

        if (!receptionist.IsActive)
        {
            return ReceptionistErrors.NotActive(receptionist.Id);
        }

        var trimmedPhone = request.PhoneNumber.Trim();
        var hasSamePhone = string.Equals(receptionist.PhoneNumber, trimmedPhone, StringComparison.InvariantCultureIgnoreCase);
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
        var hasSameEmail = string.Equals(receptionist.Email, trimmedEmail, StringComparison.InvariantCultureIgnoreCase);
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
        receptionist.Update(trimmedFirstName, trimmedLastName, 
            trimmedMiddleName, trimmedPhone, trimmedEmail);

        await receptionistRepository.UpdateAsync(receptionist, cancellationToken);

        return Result.Success();
    }
}