using Logitar;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace Krakenar.Infrastructure.Passwords.Pbkdf2;

public record Pbkdf2Settings
{
  public const string SectionKey = "Passwords:Pbkdf2";

  public KeyDerivationPrf Algorithm { get; set; } = KeyDerivationPrf.HMACSHA256;
  public int Iterations { get; set; } = 600000;
  public int SaltLength { get; set; } = 256 / 8;
  public int? HashLength { get; set; }

  public static Pbkdf2Settings Initialize(IConfiguration configuration)
  {
    Pbkdf2Settings settings = configuration.GetSection(SectionKey).Get<Pbkdf2Settings>() ?? new();

    settings.Algorithm = EnvironmentHelper.GetEnum("PASSWORDS_PBKDF2_ALGORITHM", settings.Algorithm);
    settings.Iterations = EnvironmentHelper.GetInt32("PASSWORDS_PBKDF2_ITERATIONS", settings.Iterations);
    settings.SaltLength = EnvironmentHelper.GetInt32("PASSWORDS_PBKDF2_SALT_LENGTH", settings.SaltLength);
    settings.HashLength = EnvironmentHelper.TryGetInt32("PASSWORDS_PBKDF2_HASH_LENGTH") ?? settings.HashLength;

    return settings;
  }
}
