namespace Krakenar.Contracts.Settings;

public interface IUniqueNameSettings
{
  string? AllowedCharacters { get; }
}

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

  public UniqueNameSettings()
  {
  }

  public UniqueNameSettings(IUniqueNameSettings settings) : this(settings.AllowedCharacters)
  {
  }

  public UniqueNameSettings(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
  }
}
