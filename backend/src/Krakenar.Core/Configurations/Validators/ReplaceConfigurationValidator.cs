using FluentValidation;
using Krakenar.Contracts.Configurations;
using Krakenar.Core.Settings.Validators;

namespace Krakenar.Core.Configurations.Validators;

public class ReplaceConfigurationValidator : AbstractValidator<ReplaceConfigurationPayload>
{
  public ReplaceConfigurationValidator()
  {
    RuleFor(x => x.UniqueNameSettings).SetValidator(new UniqueNameSettingsValidator());
    RuleFor(x => x.PasswordSettings).SetValidator(new PasswordSettingsValidator());
    RuleFor(x => x.LoggingSettings).SetValidator(new LoggingSettingsValidator());
  }
}
