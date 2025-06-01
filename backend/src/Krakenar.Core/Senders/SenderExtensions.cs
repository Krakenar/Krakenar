using Krakenar.Contracts.Senders;
using Krakenar.Core.Encryption;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders.Settings;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders;

public static class SenderExtensions
{
  public static void Decrypt(this IEncryptionManager encryptionManager, SenderDto sender) // TODO(fpion): tests
  {
    RealmId? realmId = sender.Realm is null ? null : new(sender.Realm.Id);
    if (sender.SendGrid is not null)
    {
      sender.SendGrid.ApiKey = encryptionManager.Decrypt(new EncryptedString(sender.SendGrid.ApiKey), realmId);
    }
    if (sender.Twilio is not null)
    {
      sender.Twilio.AccountSid = encryptionManager.Decrypt(new EncryptedString(sender.Twilio.AccountSid), realmId);
      sender.Twilio.AuthenticationToken = encryptionManager.Decrypt(new EncryptedString(sender.Twilio.AuthenticationToken), realmId);
    }
  }

  public static SenderKind GetSenderKind(this SenderSettings settings) => settings.Provider.GetSenderKind();
  public static SenderKind GetSenderKind(this SenderProvider provider) => provider switch
  {
    SenderProvider.SendGrid => SenderKind.Email,
    SenderProvider.Twilio => SenderKind.Phone,
    _ => throw new SenderProviderNotSupportedException(provider),
  };
}
