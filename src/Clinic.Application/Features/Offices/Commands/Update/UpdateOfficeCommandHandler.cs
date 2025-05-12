using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Update;
public sealed class UpdateOfficeCommandHandler(
    IOfficeRepository officeRepository,
    UpdateOfficeCommandValidator validator
    ) : IRequestHandler<UpdateOfficeCommand, Result>
{
    public async Task<Result> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
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

        var trimmedPhone = request.PhoneNumber.Trim();
        if (office.PhoneNumber != trimmedPhone)
        {
            if (await officeRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken))
            {
                return OfficeErrors.PhoneAlreadyTaken(trimmedPhone);
            }
        }

        office.Update(request.Address, trimmedPhone);

        await officeRepository.UpdateAsync(office, cancellationToken);

        return Result.Success();
    }
}