using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Add;
public sealed class AddOfficeCommandHandler(
    IOfficeRepository officeRepository,
    AddOfficeCommandValidator validator
    ) : IRequestHandler<AddOfficeCommand, Result>
{
    public async Task<Result> Handle(AddOfficeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var trimmedPhone = request.PhoneNumber.Trim();
        if (await officeRepository.ExistsByPhoneNumberAsync(trimmedPhone, cancellationToken))
        {
            return OfficeErrors.PhoneAlreadyTaken(request.PhoneNumber);
        }

        var office = Domain.Office.Office.Create(request.Address, trimmedPhone);

        await officeRepository.AddAsync(office, cancellationToken);

        return Result.Success();
    }
}