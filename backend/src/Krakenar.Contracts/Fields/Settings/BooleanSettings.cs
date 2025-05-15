namespace Krakenar.Contracts.Fields.Settings;

public record BooleanSettings : IBooleanSettings
{
  [JsonConstructor]
  public BooleanSettings()
  {
  }

  public BooleanSettings(IBooleanSettings _) : this()
  {
  }
}
