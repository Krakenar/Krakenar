using FluentValidation;
using Krakenar.Contracts.Localization;

namespace Krakenar.Core.Localization.Validators;

public class UpdateLanguageValidator : AbstractValidator<UpdateLanguagePayload>
{
  public UpdateLanguageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
  }
}
