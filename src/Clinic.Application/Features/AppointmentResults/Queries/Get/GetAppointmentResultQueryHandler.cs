using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Queries.Get;

public sealed class GetAppointmentResultQueryHandler(IAppointmentResultRepository appointmentResultRepository)
    : IRequestHandler<GetAppointmentResultQuery, Result<AppointmentResultResponse>>
{
    public async Task<Result<AppointmentResultResponse>> Handle(GetAppointmentResultQuery request, CancellationToken cancellationToken)
    {
        var appointmentResult = await appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointmentResult is null)
        {
            return Result.Failure<AppointmentResultResponse>(
                AppointmentResultErrors.NotFound(request.Id));
        }

        return AppointmentResultResponse.FromAppointmentResult(appointmentResult);
    }
}