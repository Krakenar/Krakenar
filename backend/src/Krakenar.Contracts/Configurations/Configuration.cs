using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Configurations;

public class Configuration : Aggregate
{
  [JsonIgnore]
  public string? Secret { get; set; }
  public Actor SecretChangedBy { get; set; } = new();
  public DateTime SecretChangedOn { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public LoggingSettings LoggingSettings { get; set; } = new();
}
