using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;

public class GetPaginatedAppointmentResultsRequestHandler(
    IUserAccessService userAccessService,
    IAppointmentResultRepository appointmentResultRepository,
    GetPaginatedAppointmentResultsRequestValidator validator)
    : IRequestHandler<GetPaginatedAppointmentResultsRequest, Result<PaginatedCollection<AppointmentResultResponse>>>
{
    public async Task<Result<PaginatedCollection<AppointmentResultResponse>>> Handle(
        GetPaginatedAppointmentResultsRequest request, CancellationToken cancellationToken)
    {
        var userResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Doctor, RoleType.Receptionist, RoleType.Patient], cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure<PaginatedCollection<AppointmentResultResponse>>(userResult.Error);
        }

        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<AppointmentResultResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedAppointmentResults = userResult.Value.Role == RoleType.Patient 
            ? await appointmentResultRepository.GetPaginatedAsync(userResult.Value.Id,
                request.AppointmentResultFilter, request.Page, request.PageSize, cancellationToken)
            : await appointmentResultRepository.GetPaginatedAsync(
                request.AppointmentResultFilter, request.Page, request.PageSize, cancellationToken);
        var appointmentResultResponse = paginatedAppointmentResults
            .Select(AppointmentResultResponse.FromAppointmentResult);
        return Result.Success(appointmentResultResponse);
    }
}