using FluentValidation;

namespace Clinic.Application.Features.AppointmentResults.Commands.Add;

public sealed class AddAppointmentResultCommandValidator : AbstractValidator<AddAppointmentResultCommand>
{
    public AddAppointmentResultCommandValidator()
    {
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