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

    string? algorithmValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_ALGORITHM");
    if (!string.IsNullOrWhiteSpace(algorithmValue) && Enum.TryParse(algorithmValue, out KeyDerivationPrf algorithm) && Enum.IsDefined(algorithm))
    {
      settings.Algorithm = algorithm;
    }

    string? iterationsValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_ITERATIONS");
    if (!string.IsNullOrWhiteSpace(iterationsValue) && int.TryParse(iterationsValue, out int iterations))
    {
      settings.Iterations = iterations;
    }

    string? saltLengthValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_SALT_LENGTH");
    if (!string.IsNullOrWhiteSpace(saltLengthValue) && int.TryParse(saltLengthValue, out int saltLength))
    {
      settings.SaltLength = saltLength;
    }

    string? hashLengthValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_HASH_LENGTH");
    if (!string.IsNullOrWhiteSpace(hashLengthValue) && int.TryParse(hashLengthValue, out int hashLength))
    {
      settings.HashLength = hashLength;
    }

    return settings;
  }
}
