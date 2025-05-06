using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Clinic.Application.Validation;
public partial class PhoneNumberValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(PhoneNumberValidator<T>);

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (!PhoneNumberRegex().IsMatch(value))
        {
            context.AddFailure(context.PropertyPath, $"'{context.PropertyPath}' is not a valid phone number. Example: 291234567; 331234567; 441234567; 251234567.");
        }
        else
        {
            return true;
        }

        return false;
    }


    [GeneratedRegex(@"^(33|29|44|25)\d{7}$")]
    private static partial Regex PhoneNumberRegex();
}