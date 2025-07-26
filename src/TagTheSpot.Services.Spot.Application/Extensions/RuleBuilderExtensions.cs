using FluentValidation;

namespace TagTheSpot.Services.Spot.Application.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeEnumValue<T, TEnum>(
            this IRuleBuilder<T, string> ruleBuilder,
            bool ignoreCase = true
        ) where TEnum : struct, Enum
        {
            return ruleBuilder
                .Must(value =>
                    Enum.TryParse<TEnum>(value, ignoreCase, out var parsed)
                    && Enum.IsDefined(typeof(TEnum), parsed))
                .WithMessage("'{PropertyValue}' is not a valid value for {PropertyName}.");
        }
    }
}
