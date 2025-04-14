using FluentValidation;
using Krakenar.Contracts.Configurations;
using Krakenar.Core.Settings.Validators;

namespace Krakenar.Core.Configurations.Validators;

public class UpdateConfigurationValidator : AbstractValidator<UpdateConfigurationPayload>
{
  public UpdateConfigurationValidator()
  {
    When(x => x.UniqueNameSettings is not null, () => RuleFor(x => x.UniqueNameSettings!).SetValidator(new UniqueNameSettingsValidator()));
    When(x => x.PasswordSettings is not null, () => RuleFor(x => x.PasswordSettings!).SetValidator(new PasswordSettingsValidator()));
    When(x => x.LoggingSettings is not null, () => RuleFor(x => x.LoggingSettings!).SetValidator(new LoggingSettingsValidator()));
  }
}
