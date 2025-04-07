using FluentValidation;
using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Settings.Validators;

public class UniqueNameSettingsValidator : AbstractValidator<IUniqueNameSettings>
{
  public UniqueNameSettingsValidator()
  {
    When(x => x.AllowedCharacters is not null, () => RuleFor(x => x.AllowedCharacters).NotEmpty().MaximumLength(byte.MaxValue));
  }
}
