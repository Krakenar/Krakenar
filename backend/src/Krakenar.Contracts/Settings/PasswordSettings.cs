namespace Krakenar.Contracts.Settings;

public interface IPasswordSettings
{
  int RequiredLength { get; }
  int RequiredUniqueChars { get; }
  bool RequireNonAlphanumeric { get; }
  bool RequireLowercase { get; }
  bool RequireUppercase { get; }
  bool RequireDigit { get; }
  string HashingStrategy { get; }
}

public record PasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; set; } = 8;
  public int RequiredUniqueChars { get; set; } = 8;
  public bool RequireNonAlphanumeric { get; set; } = true;
  public bool RequireLowercase { get; set; } = true;
  public bool RequireUppercase { get; set; } = true;
  public bool RequireDigit { get; set; } = true;
  public string HashingStrategy { get; set; } = "PBKDF2";

  public PasswordSettings()
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
    HashingStrategy = hashingStrategy;
  }
}
