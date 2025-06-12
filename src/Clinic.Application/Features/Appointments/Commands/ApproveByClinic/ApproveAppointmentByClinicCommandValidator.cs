using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.ApproveByClinic;

public sealed class ApproveAppointmentByClinicCommandValidator 
    : AbstractValidator<ApproveAppointmentByClinicCommand>
{
    public ApproveAppointmentByClinicCommandValidator()
    {
        RuleFor(a => a.AppointmentId)
            .NotEmpty();
    }
}