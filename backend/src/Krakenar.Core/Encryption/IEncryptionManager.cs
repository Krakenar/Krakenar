using Krakenar.Core.Realms;

namespace Krakenar.Core.Encryption;

public interface IEncryptionManager
{
  string Decrypt(EncryptedString encrypted, RealmId? realmId = null);
  EncryptedString Encrypt(string decrypted, RealmId? realmId = null);
}
