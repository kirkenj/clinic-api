using FluentValidation;

namespace Clinic.Application.Validation;
public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<T, string> BelarusianPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(@"^(33|29|44|25)\d{7}$")
            .WithMessage("'{PropertyPath}' is not a valid phone number. Example: 291234567; 331234567; 441234567; 251234567.");
    }

    public static IRuleBuilderOptions<T, string> Address<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MinimumLength(10);
    }

    public static IRuleBuilderOptions<T, string> MaxTrimmedLength<T>(this IRuleBuilder<T, string> ruleBuilder, int maxTrimmedLength)
    {
        return ruleBuilder.Must(v => v?.Trim().Length <= maxTrimmedLength)
            .WithMessage($"'{{PropertyPath}}' must be at most {maxTrimmedLength} characters after trimming.");
    }

    public static IRuleBuilderOptions<T, string> TitleField<T>(this IRuleBuilder<T, string> ruleBuilder, int maxTrimmedLength = 20)
    {
        return ruleBuilder
            .NotEmpty()
            .MaxTrimmedLength(maxTrimmedLength);
    }
}