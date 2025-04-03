using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Validators;

namespace Krakenar.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> AllowedCharacters<T>(this IRuleBuilder<T, string> ruleBuilder, string? allowedCharacters)
  {
    return ruleBuilder.SetValidator(new AllowedCharactersValidator<T>(allowedCharacters));
  }

  public static IRuleBuilderOptions<T, string> Locale<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Localization.Locale.MaximumLength).SetValidator(new LocaleValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> UniqueName<T>(this IRuleBuilder<T, string> ruleBuilder, IUniqueNameSettings uniqueNameSettings)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.UniqueName.MaximumLength).AllowedCharacters(uniqueNameSettings.AllowedCharacters);
  }
}
