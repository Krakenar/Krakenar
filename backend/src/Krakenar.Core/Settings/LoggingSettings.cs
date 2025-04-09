using FluentValidation;
using Krakenar.Contracts.Logging;
using Krakenar.Core.Settings.Validators;

namespace Krakenar.Core.Settings;

public record LoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; }
  public bool OnlyErrors { get; }

  public LoggingSettings() : this(LoggingExtent.ActivityOnly)
  {
  }

  public LoggingSettings(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  [JsonConstructor]
  public LoggingSettings(LoggingExtent extent, bool onlyErrors = false)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
    new LoggingSettingsValidator().ValidateAndThrow(this);
  }
}
