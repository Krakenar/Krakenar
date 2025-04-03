using FluentValidation;
using Krakenar.Core.Validators;

namespace Krakenar.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Locale<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Localization.Locale.MaximumLength).SetValidator(new LocaleValidator<T>());
  }
}
