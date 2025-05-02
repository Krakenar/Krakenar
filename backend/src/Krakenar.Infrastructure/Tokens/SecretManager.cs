using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Krakenar.Infrastructure.Settings;
using Logitar;
using Logitar.Security.Cryptography;

namespace Krakenar.Infrastructure.Tokens;

public class SecretManager : ISecretManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual EncryptionSettings Settings { get; }

  public SecretManager(IApplicationContext applicationContext, EncryptionSettings settings)
  {
    ApplicationContext = applicationContext;
    Settings = settings;
  }

  public string Decrypt(Secret secret, RealmId? realmId)
  {
    byte[] bytes = Convert.FromBase64String(secret.Value);
    byte length = bytes.First();
    byte[] iv = bytes.Skip(1).Take(length).ToArray();
    byte[] encryptedBytes = bytes.Skip(1 + length).ToArray();

    using Aes aes = Aes.Create();
    using ICryptoTransform decryptor = aes.CreateDecryptor(GetEncryptionKey(realmId), iv);
    using MemoryStream encryptedStream = new(encryptedBytes);
    using CryptoStream cryptoStream = new(encryptedStream, decryptor, CryptoStreamMode.Read);

    using MemoryStream decryptedStream = new();
    cryptoStream.CopyTo(decryptedStream);
    return Encoding.UTF8.GetString(decryptedStream.ToArray());
  }

  public virtual Secret Encrypt(string secret, RealmId? realmId)
  {
    secret = secret.Remove(" ");
    if (secret.Length < Secret.MinimumLength || secret.Length > Secret.MaximumLength)
    {
      throw new ArgumentException($"The secret must be between {Secret.MinimumLength} and {Secret.MaximumLength} characters long (inclusive), excluding any spaces.");
    }
    byte[] data = Encoding.UTF8.GetBytes(secret);

    using Aes aes = Aes.Create();
    using ICryptoTransform encryptor = aes.CreateEncryptor(GetEncryptionKey(realmId), aes.IV);
    using MemoryStream encryptedStream = new();
    using CryptoStream cryptoStream = new(encryptedStream, encryptor, CryptoStreamMode.Write);
    cryptoStream.Write(data, 0, data.Length);
    cryptoStream.FlushFinalBlock();
    byte[] encryptedBytes = encryptedStream.ToArray();

    byte length = (byte)aes.IV.Length;
    string encrypted = Convert.ToBase64String(new byte[] { length }.Concat(aes.IV).Concat(encryptedBytes).ToArray());
    return new Secret(encrypted);
  }

  public virtual Secret Generate(RealmId? realmId = null)
  {
    string secret = RandomStringGenerator.GetString(Secret.MinimumLength);
    return Encrypt(secret, realmId);
  }

  public virtual string Resolve(string? value = null)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return Decrypt(ApplicationContext.Secret, ApplicationContext.RealmId);
    }

    return value.Trim();
  }

  protected virtual byte[] GetEncryptionKey(RealmId? realmId)
  {
    byte[] key = HKDF.Extract(HashAlgorithmName.SHA256, Encoding.UTF8.GetBytes(Settings.Key));
    if (realmId.HasValue)
    {
      key = HKDF.Expand(HashAlgorithmName.SHA256, key, key.Length, realmId.Value.ToGuid().ToByteArray());
    }
    return key;
  }
}
