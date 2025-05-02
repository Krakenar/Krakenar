using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Passwords;
using Logitar.Security.Cryptography;

namespace Krakenar.Infrastructure.Passwords;

public class PasswordManager : IPasswordManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual Dictionary<string, IPasswordStrategy> Strategies { get; } = [];

  public PasswordManager(IApplicationContext applicationContext, IEnumerable<IPasswordStrategy> strategies)
  {
    ApplicationContext = applicationContext;

    foreach (IPasswordStrategy strategy in strategies)
    {
      Strategies[strategy.Key] = strategy;
    }
  }

  public PasswordManager(IApplicationContext applicationContext)
  {
    ApplicationContext = applicationContext;
  }

  public virtual Password Decode(string password)
  {
    string key = password.Split(Password.Separator).First();
    return GetStrategy(key).Decode(password);
  }

  public virtual Password Generate(int length, out string password)
  {
    password = RandomStringGenerator.GetString(length);
    return Hash(password);
  }
  public virtual Password Generate(string characters, int length, out string password)
  {
    password = RandomStringGenerator.GetString(characters, length);
    return Hash(password);
  }

  public virtual Password GenerateBase64(int length, out string password)
  {
    password = RandomStringGenerator.GetBase64String(length, out _);
    return Hash(password);
  }

  public virtual Password Hash(string password) => Hash(password, settings: null);
  protected virtual Password Hash(string password, IPasswordSettings? settings)
  {
    settings ??= ApplicationContext.PasswordSettings;
    return GetStrategy(settings.HashingStrategy).Hash(password);
  }

  public virtual void Validate(string password) => Validate(password, settings: null);
  protected virtual void Validate(string password, IPasswordSettings? settings)
  {
    settings ??= ApplicationContext.PasswordSettings;
    _ = new PasswordInput(settings, password);
  }

  public virtual Password ValidateAndHash(string password, IPasswordSettings? settings)
  {
    Validate(password, settings);
    return Hash(password, settings);
  }

  protected virtual IPasswordStrategy GetStrategy(string key)
  {
    return Strategies.TryGetValue(key, out IPasswordStrategy? strategy) ? strategy : throw new PasswordStrategyNotSupportedException(key);
  }
}
