using FluentValidation;
using Krakenar.Contracts.Logging;

namespace Krakenar.Core.Settings.Validators;

public class LoggingSettingsValidator : AbstractValidator<ILoggingSettings>
{
  public LoggingSettingsValidator()
  {
    RuleFor(x => x.Extent).IsInEnum();
    When(x => x.Extent == LoggingExtent.None, () => RuleFor(x => x.OnlyErrors).Equal(false));
  }
}
