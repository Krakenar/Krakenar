using FluentValidation;
using Krakenar.Contracts.Localization;

namespace Krakenar.Core.Localization.Validators;

public class CreateOrReplaceLanguageValidator : AbstractValidator<CreateOrReplaceLanguagePayload>
{
  public CreateOrReplaceLanguageValidator()
  {
    RuleFor(x => x.Locale).Locale();
  }
}
