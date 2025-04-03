using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Configurations;

public class ConfigurationModel : AggregateModel
{
  public UniqueNameSettingsModel UniqueNameSettings { get; set; } = new();
  public PasswordSettingsModel PasswordSettings { get; set; } = new();
}
