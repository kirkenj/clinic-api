using FluentValidation;

namespace Clinic.Application.Features.AppointmentResults.Commands.Update;

public sealed class UpdateAppointmentResultCommandValidator : AbstractValidator<UpdateAppointmentResultCommand>
{
    public UpdateAppointmentResultCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
        RuleFor(c => c.AppointmentId)
            .NotEmpty();
        RuleFor(m => m.Complaints)
            .NotEmpty();
        RuleFor(m => m.Recommendations)
            .NotEmpty();
        RuleFor(m => m.Conclusion)
            .NotEmpty();
    }
}