using Krakenar.Core.Passwords;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Krakenar.Infrastructure.Passwords.Pbkdf2;

public record Pbkdf2Password : Password
{
  public const string Key = "PBKDF2";

  private readonly KeyDerivationPrf _algorithm;
  private readonly int _iterations;
  private readonly byte[] _salt;
  private readonly byte[] _hash;

  public Pbkdf2Password(string password, KeyDerivationPrf algorithm, int iterations, int saltLength, int? hashLength = null)
  {
    _algorithm = algorithm;
    _iterations = iterations;
    _salt = RandomNumberGenerator.GetBytes(saltLength);
    _hash = ComputeHash(password, hashLength ?? saltLength);
  }

  private Pbkdf2Password(KeyDerivationPrf algorithm, int iterations, byte[] salt, byte[] hash)
  {
    _algorithm = algorithm;
    _iterations = iterations;
    _salt = salt;
    _hash = hash;
  }

  public static Pbkdf2Password Decode(string password)
  {
    string[] values = password.Split(Separator);
    if (values.Length != 5 || values.First() != Key)
    {
      throw new ArgumentException($"The value '{password}' is not a valid PBKDF2 password.", nameof(password));
    }

    return new Pbkdf2Password(Enum.Parse<KeyDerivationPrf>(values[1]), int.Parse(values[2]),
      Convert.FromBase64String(values[3]), Convert.FromBase64String(values[4]));
  }

  public override string Encode() => string.Join(Separator, Key, _algorithm, _iterations,
    Convert.ToBase64String(_salt), Convert.ToBase64String(_hash));

  public override bool IsMatch(string password) => _hash.SequenceEqual(ComputeHash(password));

  private byte[] ComputeHash(string password, int? length = null)
  {
    return KeyDerivation.Pbkdf2(password, _salt, _algorithm, _iterations, length ?? _hash.Length);
  }
}
