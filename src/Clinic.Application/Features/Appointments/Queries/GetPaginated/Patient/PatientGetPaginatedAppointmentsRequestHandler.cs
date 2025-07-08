using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated.Patient;

public class PatientGetPaginatedAppointmentsRequestHandler(
    IAppointmentService appointmentService,
    IUserAccessService userAccessService,
    PatientGetPaginatedAppointmentsRequestValidator validator) : IRequestHandler<PatientGetPaginatedAppointmentsRequest,
    Result<PaginatedCollection<AppointmentResponse>>>
{
    public async Task<Result<PaginatedCollection<AppointmentResponse>>> Handle(PatientGetPaginatedAppointmentsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<AppointmentResponse>>
                (Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Patient], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<PaginatedCollection<AppointmentResponse>>(
                userAccessResult.Error);
        }

        var filter = request.AppointmentFilter?.ToAppointmentFilter() ?? new();
        filter.PatientIds = [userAccessResult.Value.Id];
        return await appointmentService.GetPaginatedAsync(filter,
            request.Page, request.PageSize, cancellationToken);
    }
}