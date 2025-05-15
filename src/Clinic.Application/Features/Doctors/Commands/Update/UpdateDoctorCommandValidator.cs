using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Doctors.Commands.Update;
public sealed class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .TitleField();
        RuleFor(c => c.LastName)
            .TitleField();
        RuleFor(c => c.MiddleName)
            .TitleField();
        RuleFor(c => c.PhoneNumber)
            .BelarusianPhoneNumber();
        RuleFor(m => m.Email)
            .EmailAddress();
        RuleFor(c => c.SpecializationId)
            .NotEmpty();
        RuleFor(c => c.OfficeId)
            .NotEmpty();
    }
}