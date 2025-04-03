using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Settings.Validators;

namespace Krakenar.Core.Settings;

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; }

  public UniqueNameSettings() : this("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+")
  {
  }

  public UniqueNameSettings(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }

  [JsonConstructor]
  public UniqueNameSettings(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
    new UniqueNameSettingsValidator().ValidateAndThrow(this);
  }
}
