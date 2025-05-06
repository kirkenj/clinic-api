using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Add;
public sealed class AddOfficeCommandHandler(IOfficeRepository officeRepository,
    AddOfficeCommandValidator validator) : IRequestHandler<AddOfficeCommand, Result>
{
    public async Task<Result> Handle(AddOfficeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        if (await officeRepository.ExcistsByAddressAsync(request.Address, cancellationToken))
        {
            return OfficeErrors.AddressAlreadyTaken(request.Address);
        }

        var officeToAdd = Domain.Office.Office.Create(request.Address, request.RegistryPhoneNumber, request.Photo, request.PhotoId);

        await officeRepository.AddAsync(officeToAdd, cancellationToken);

        return Result.Success();
    }
}