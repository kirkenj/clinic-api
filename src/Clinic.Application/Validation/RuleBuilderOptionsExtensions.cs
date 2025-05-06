using FluentValidation;

namespace Clinic.Application.Validation;
public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<T, string> BelarusianPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new PhoneNumberValidator<T>())
            .WithMessage("$'{PropertyPath}' is not a valid phone number. Example: 291234567; 331234567; 441234567; 251234567.");
    }

    public static IRuleBuilderOptions<T, string> Address<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MinimumLength(10);
    }
}