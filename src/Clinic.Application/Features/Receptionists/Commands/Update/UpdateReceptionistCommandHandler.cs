using Clinic.Domain.Abstractions;
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
        if (receptionist.PhoneNumber != trimmedPhone)
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
        if (receptionist.Email != trimmedEmail)
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

        receptionist.Update(request.FirstName, request.LastName, request.MiddleName,
            trimmedEmail, trimmedPhone);

        await receptionistRepository.UpdateAsync(receptionist, cancellationToken);

        return Result.Success();
    }
}