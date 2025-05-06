using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Update;
public sealed class UpdateOfficeCommandHandler(IOfficeRepository officeRepository,
    UpdateOfficeCommandValidator validator) : IRequestHandler<UpdateOfficeCommand, Result>
{
    public async Task<Result> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var officeToUpdate = await officeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (officeToUpdate == null)
        {
            return OfficeErrors.NotFound(request.Id);
        }

        if (officeToUpdate.Address != request.Address && await officeRepository.ExcistsByAddressAsync(request.Address, cancellationToken))
        {
            return OfficeErrors.AddressAlreadyTaken(request.Address);
        }

        officeToUpdate.Update(request.Address, request.RegistryPhoneNumber, request.Photo, request.PhotoId);

        await officeRepository.UpdateAsync(officeToUpdate, cancellationToken);

        return Result.Success();
    }
}