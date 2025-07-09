using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Update;

public sealed class UpdateOfficeCommandHandler
    (IOfficeRepository officeRepository,
    IUserAccessService userAccessService,
    UpdateOfficeCommandValidator validator)
    : IRequestHandler<UpdateOfficeCommand, Result>
{
    public async Task<Result> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        var office = await officeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (office is null)
        {
            return OfficeErrors.NotFound(request.Id);
        }

        if (!office.IsActive)
        {
            return OfficeErrors.NotActive(office.Id);
        }

        var hasSamePhone = office.PhoneNumber.HasSameValue(request.PhoneNumber);
        if (!hasSamePhone)
        {
            var isPhoneTaken = await officeRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (isPhoneTaken)
            {
                return OfficeErrors.PhoneAlreadyTaken(request.PhoneNumber);
            }
        }

        var hasSameAddress = office.Address.HasSameValue(request.Address);
        if (!hasSameAddress)
        {
            var isAddressTaken = await officeRepository.ExistsByAddressAsync(request.Address, cancellationToken);
            if (isAddressTaken)
            {
                return OfficeErrors.AddressAlreadyTaken(request.Address);
            }
        }

        office.Update(request.Address, request.PhoneNumber);
        await officeRepository.UpdateAsync(office, cancellationToken);
        return Result.Success();
    }
}