using Krakenar.Contracts.Senders.Settings;
using Krakenar.Core.Encryption;
using Krakenar.Core.Realms;

namespace Krakenar.Core.Senders.Settings;

public static class SenderSettingsExtensions
{
  public static bool AreEqual(this ISendGridSettings encrypted, ISendGridSettings decrypted, IEncryptionManager encryptionManager, RealmId? realmId)
  {
    string apiKey = encryptionManager.Decrypt(new EncryptedString(encrypted.ApiKey), realmId);
    return apiKey == decrypted.ApiKey;
  }

  public static bool AreEqual(this ISmtpProviderSettings encrypted, ISmtpProviderSettings decrypted, IEncryptionManager encryptionManager, RealmId? realmId)
  {
    string username = encryptionManager.Decrypt(new EncryptedString(encrypted.Username), realmId);
    string password = encryptionManager.Decrypt(new EncryptedString(encrypted.Password), realmId);
    return encrypted.Host == decrypted.Host
      && encrypted.Port == decrypted.Port
      && encrypted.Security == decrypted.Security
      && username == decrypted.Username
      && password == decrypted.Password;
  }

  public static bool AreEqual(this ITwilioSettings encrypted, ITwilioSettings decrypted, IEncryptionManager encryptionManager, RealmId? realmId)
  {
    string accountSid = encryptionManager.Decrypt(new EncryptedString(encrypted.AccountSid), realmId);
    string authenticationToken = encryptionManager.Decrypt(new EncryptedString(encrypted.AuthenticationToken), realmId);
    return accountSid == decrypted.AccountSid && authenticationToken == decrypted.AuthenticationToken;
  }
}
