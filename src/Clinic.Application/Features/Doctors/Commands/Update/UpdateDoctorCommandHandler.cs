using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Update;

public sealed class UpdateDoctorCommandHandler
    (IDoctorService doctorService,
    IUserAccessService userAccessService,
    UpdateDoctorCommandValidator validator)
    : IRequestHandler<UpdateDoctorCommand, Result>
{
    public async Task<Result> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync([RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        return await doctorService.UpdateAsync(request.Id, request.FirstName, request.MiddleName,
            request.LastName, request.PhoneNumber, request.Email, request.SpecializationId,
            request.OfficeId, cancellationToken);
    }
}