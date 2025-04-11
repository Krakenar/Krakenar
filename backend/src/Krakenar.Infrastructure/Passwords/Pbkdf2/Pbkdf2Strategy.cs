using Krakenar.Core.Passwords;

namespace Krakenar.Infrastructure.Passwords.Pbkdf2;

public class Pbkdf2Strategy : IPasswordStrategy
{
  protected virtual Pbkdf2Settings Settings { get; }

  public virtual string Key => Pbkdf2Password.Key;

  public Pbkdf2Strategy(Pbkdf2Settings settings)
  {
    Settings = settings;
  }

  public virtual Password Decode(string password) => Pbkdf2Password.Decode(password);

  public virtual Password Hash(string password) => new Pbkdf2Password(password, Settings.Algorithm, Settings.Iterations, Settings.SaltLength, Settings.HashLength);
}
