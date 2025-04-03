namespace Krakenar.Contracts.Settings;

public record UniqueNameSettingsModel : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

  public UniqueNameSettingsModel()
  {
  }

  public UniqueNameSettingsModel(IUniqueNameSettings settings) : this(settings.AllowedCharacters)
  {
  }

  public UniqueNameSettingsModel(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
  }
}
