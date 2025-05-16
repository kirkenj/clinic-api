using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Add;

public sealed class AddOfficeCommandHandler
    (IOfficeRepository officeRepository,
        AddOfficeCommandValidator validator)
    : IRequestHandler<AddOfficeCommand, Result>
{
    public async Task<Result> Handle(AddOfficeCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var isPhoneTaken = await officeRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (isPhoneTaken)
        {
            return OfficeErrors.PhoneAlreadyTaken(request.PhoneNumber);
        }

        var office = Office.Create(request.Address, request.PhoneNumber);

        await officeRepository.AddAsync(office, cancellationToken);

        return Result.Success();
    }
}