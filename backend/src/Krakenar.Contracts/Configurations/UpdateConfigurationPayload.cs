using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Configurations;

public record UpdateConfigurationPayload
{
  public Change<string>? Secret { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public LoggingSettings? LoggingSettings { get; set; }
}
