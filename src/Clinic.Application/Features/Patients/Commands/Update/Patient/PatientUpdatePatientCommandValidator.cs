using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.Update.Patient;

public sealed class PatientUpdatePatientCommandValidator : AbstractValidator<PatientUpdatePatientCommand>
{
    public PatientUpdatePatientCommandValidator()
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
    }
}