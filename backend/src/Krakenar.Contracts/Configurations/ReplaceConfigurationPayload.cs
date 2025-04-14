using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Configurations;

public record ReplaceConfigurationPayload
{
  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public LoggingSettings LoggingSettings { get; set; } = new();
}
