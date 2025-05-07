using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Update;
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
        if (office == null)
        {
            return OfficeErrors.NotFound(request.Id);
        }

        if (office.PhoneNumber != request.PhoneNumber 
            && await officeRepository.ExistsByPhoneNumber(request.PhoneNumber, cancellationToken))
        {
            return OfficeErrors.PhoneAlreadyTaken(request.PhoneNumber);
        }

        office.Update(request.Address, request.PhoneNumber);

        await officeRepository.UpdateAsync(office, cancellationToken);

        return Result.Success();
    }
}