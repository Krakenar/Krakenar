using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Settings.Validators;

namespace Krakenar.Core.Settings;

public record PasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; }
  public int RequiredUniqueChars { get; }
  public bool RequireNonAlphanumeric { get; }
  public bool RequireLowercase { get; }
  public bool RequireUppercase { get; }
  public bool RequireDigit { get; }
  public string HashingStrategy { get; }

  public PasswordSettings() : this(
    requiredLength: 8,
    requiredUniqueChars: 8,
    requireNonAlphanumeric: true,
    requireLowercase: true,
    requireUppercase: true,
    requireDigit: true,
    hashingStrategy: "PBKDF2")
  {
  }

  public PasswordSettings(IPasswordSettings password) : this(
    password.RequiredLength,
    password.RequiredUniqueChars,
    password.RequireNonAlphanumeric,
    password.RequireLowercase,
    password.RequireUppercase,
    password.RequireDigit,
    password.HashingStrategy)
  {
  }

  [JsonConstructor]
  public PasswordSettings(
    int requiredLength,
    int requiredUniqueChars,
    bool requireNonAlphanumeric,
    bool requireLowercase,
    bool requireUppercase,
    bool requireDigit,
    string hashingStrategy)
  {
    RequiredLength = requiredLength;
    RequiredUniqueChars = requiredUniqueChars;
    RequireNonAlphanumeric = requireNonAlphanumeric;
    RequireLowercase = requireLowercase;
    RequireUppercase = requireUppercase;
    RequireDigit = requireDigit;
    HashingStrategy = hashingStrategy.Trim();
    new PasswordSettingsValidator().ValidateAndThrow(this);
  }
}
