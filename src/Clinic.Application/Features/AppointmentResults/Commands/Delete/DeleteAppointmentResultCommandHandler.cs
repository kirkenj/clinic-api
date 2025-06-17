using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Commands.Delete;

public sealed class DeleteAppointmentResultCommandHandler(IAppointmentResultRepository appointmentResultRepository)
    : IRequestHandler<DeleteAppointmentResultCommand, Result>
{
    public async Task<Result> Handle(DeleteAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        var appointmentResult = await appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointmentResult is null)
        {
            return AppointmentResultErrors.NotFound(request.Id);
        }

        await appointmentResultRepository.DeleteAsync(appointmentResult, cancellationToken);
        return Result.Success();
    }
}