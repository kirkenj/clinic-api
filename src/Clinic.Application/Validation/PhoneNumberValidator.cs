using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Clinic.Application.Validation;
public partial class PhoneNumberValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(PhoneNumberValidator<T>);

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return PhoneNumberRegex().IsMatch(value);
    }

    [GeneratedRegex(@"^(33|29|44|25)\d{7}$")]
    private static partial Regex PhoneNumberRegex();
}