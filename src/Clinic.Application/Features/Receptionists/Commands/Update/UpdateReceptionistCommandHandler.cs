using Clinic.Application.Extensions;
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
        request.TrimStringProperties();

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

        var hasSamePhone = receptionist.PhoneNumber.HasSameValue(request.PhoneNumber);
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

        var hasSameEmail = receptionist.Email.HasSameValue(request.Email);
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


        receptionist.Update(request.FirstName, request.LastName,
            request.MiddleName, request.PhoneNumber, request.Email);

        await receptionistRepository.UpdateAsync(receptionist, cancellationToken);

        return Result.Success();
    }
}