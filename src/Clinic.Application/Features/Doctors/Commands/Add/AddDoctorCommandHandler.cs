using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Add;

public sealed class AddDoctorCommandHandler
    (IDoctorService doctorService,
    IUserAccessService userAccessService,
    AddDoctorCommandValidator validator)
    : IRequestHandler<AddDoctorCommand, Result>
{
    public async Task<Result> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
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

        return await doctorService.AddAsync(request.FirstName, request.MiddleName, request.LastName, 
            request.Email, request.PhoneNumber, request.OfficeId, request.SpecializationId, 
            cancellationToken);
    }
}