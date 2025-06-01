using Krakenar.Core;
using Krakenar.Core.Encryption;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Logitar;
using Logitar.Security.Cryptography;

namespace Krakenar.Infrastructure.Tokens;

public class SecretManager : ISecretManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IEncryptionManager EncryptionManager { get; }

  public SecretManager(IApplicationContext applicationContext, IEncryptionManager encryptionManager)
  {
    ApplicationContext = applicationContext;
    EncryptionManager = encryptionManager;
  }

  public string Decrypt(Secret secret, RealmId? realmId)
  {
    EncryptedString encrypted = new(secret.Value);
    return EncryptionManager.Decrypt(encrypted, realmId);
  }

  public virtual Secret Encrypt(string secret, RealmId? realmId)
  {
    secret = secret.Remove(" ");
    if (secret.Length < Secret.MinimumLength || secret.Length > Secret.MaximumLength)
    {
      throw new ArgumentException($"The secret must be between {Secret.MinimumLength} and {Secret.MaximumLength} characters long (inclusive), excluding any spaces.");
    }

    EncryptedString encrypted = EncryptionManager.Encrypt(secret, realmId);
    return new Secret(encrypted.Value);
  }

  public virtual Secret Generate(RealmId? realmId)
  {
    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    return Encrypt(secret, realmId);
  }

  public virtual string Resolve(string? value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return Decrypt(ApplicationContext.Secret, ApplicationContext.RealmId);
    }

    return value.Trim();
  }
}
