using Krakenar.Core.Encryption;
using Krakenar.Core.Realms;
using Krakenar.Infrastructure.Settings;

namespace Krakenar.Infrastructure.Encryption;

public class EncryptionManager : IEncryptionManager // TODO(fpion): tests
{
  protected virtual EncryptionSettings Settings { get; }

  public EncryptionManager(EncryptionSettings settings)
  {
    Settings = settings;
  }

  public string Decrypt(EncryptedString encrypted, RealmId? realmId)
  {
    byte[] bytes = Convert.FromBase64String(encrypted.Value);
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

  public EncryptedString Encrypt(string decrypted, RealmId? realmId)
  {
    byte[] data = Encoding.UTF8.GetBytes(decrypted);

    using Aes aes = Aes.Create();
    using ICryptoTransform encryptor = aes.CreateEncryptor(GetEncryptionKey(realmId), aes.IV);
    using MemoryStream encryptedStream = new();
    using CryptoStream cryptoStream = new(encryptedStream, encryptor, CryptoStreamMode.Write);
    cryptoStream.Write(data, 0, data.Length);
    cryptoStream.FlushFinalBlock();
    byte[] encryptedBytes = encryptedStream.ToArray();

    byte length = (byte)aes.IV.Length;
    string encrypted = Convert.ToBase64String(new byte[] { length }.Concat(aes.IV).Concat(encryptedBytes).ToArray());
    return new EncryptedString(encrypted);
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
