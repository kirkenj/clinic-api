using FluentValidation;

namespace Clinic.Application.Validation;
public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<T, string> BelarusianPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new PhoneNumberValidator<T>());
    }

    public static IRuleBuilderOptions<T, string> Address<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MinimumLength(10);
    }
}