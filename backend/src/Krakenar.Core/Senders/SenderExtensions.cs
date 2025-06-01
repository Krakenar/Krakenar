using Krakenar.Contracts.Senders;
using Krakenar.Core.Encryption;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders.Settings;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders;

public static class SenderExtensions
{
  public static void DecryptSettings(this IEncryptionManager encryptionManager, SenderDto sender) // TODO(fpion): tests
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
  public static SenderSettings DecryptSettings(this IEncryptionManager encryptionManager, Sender sender) // TODO(fpion): tests
  {
    switch (sender.Provider)
    {
      case SenderProvider.SendGrid:
        SendGridSettings sendGrid = (SendGridSettings)sender.Settings;
        return new SendGridSettings(encryptionManager.Decrypt(new EncryptedString(sendGrid.ApiKey), sender.RealmId));
      case SenderProvider.Twilio:
        TwilioSettings twilio = (TwilioSettings)sender.Settings;
        return new TwilioSettings(
          encryptionManager.Decrypt(new EncryptedString(twilio.AccountSid), sender.RealmId),
          encryptionManager.Decrypt(new EncryptedString(twilio.AuthenticationToken), sender.RealmId));
      default:
        throw new SenderProviderNotSupportedException(sender.Provider);
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
