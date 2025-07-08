using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.Appointments;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated.Staff;

public class StaffGetPaginatedAppointmentsRequestHandler(
    IUserAccessService userAccessService,
    IAppointmentService appointmentService,
    StaffGetPaginatedAppointmentsRequestValidator validator) 
    : IRequestHandler<StaffGetPaginatedAppointmentsRequest, Result<PaginatedCollection<AppointmentResponse>>>
{
    public async Task<Result<PaginatedCollection<AppointmentResponse>>> Handle(
        StaffGetPaginatedAppointmentsRequest request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<AppointmentResponse>>
                (Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            Constants.StaffRoles, cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<PaginatedCollection<AppointmentResponse>>(
                userAccessResult.Error);
        }

        return await appointmentService.GetPaginatedAsync(request.AppointmentFilter, 
            request.Page, request.PageSize, cancellationToken);
    }
}