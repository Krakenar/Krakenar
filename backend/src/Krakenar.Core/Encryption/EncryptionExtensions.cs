using Krakenar.Contracts.Senders;
using Krakenar.Core.Messages;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Settings;
using MessageDto = Krakenar.Contracts.Messages.Message;
using Sender = Krakenar.Core.Senders.Sender;
using SenderDto = Krakenar.Contracts.Senders.Sender;
using TemplateContent = Krakenar.Core.Templates.Content;
using VariableDto = Krakenar.Contracts.Messages.Variable;

namespace Krakenar.Core.Encryption;

public static class EncryptionExtensions
{
  public static void Decrypt(this IEncryptionManager manager, MessageDto message)
  {
    RealmId? realmId = message.Realm is null ? null : new(message.Realm.Id);

    message.Body.Text = manager.Decrypt(new EncryptedString(message.Body.Text), realmId);

    manager.DecryptSettings(message.Sender);

    foreach (VariableDto variable in message.Variables)
    {
      variable.Value = manager.Decrypt(new EncryptedString(variable.Value), realmId);
    }
  }

  public static void DecryptSettings(this IEncryptionManager manager, SenderDto sender)
  {
    RealmId? realmId = sender.Realm is null ? null : new(sender.Realm.Id);
    if (sender.SendGrid is not null)
    {
      sender.SendGrid.ApiKey = manager.Decrypt(new EncryptedString(sender.SendGrid.ApiKey), realmId);
    }
    if (sender.Twilio is not null)
    {
      sender.Twilio.AccountSid = manager.Decrypt(new EncryptedString(sender.Twilio.AccountSid), realmId);
      sender.Twilio.AuthenticationToken = manager.Decrypt(new EncryptedString(sender.Twilio.AuthenticationToken), realmId);
    }
  }
  public static SenderSettings DecryptSettings(this IEncryptionManager manager, Sender sender)
  {
    switch (sender.Provider)
    {
      case SenderProvider.SendGrid:
        SendGridSettings sendGrid = (SendGridSettings)sender.Settings;
        return new SendGridSettings(manager.Decrypt(new EncryptedString(sendGrid.ApiKey), sender.RealmId));
      case SenderProvider.Twilio:
        TwilioSettings twilio = (TwilioSettings)sender.Settings;
        return new TwilioSettings(
          manager.Decrypt(new EncryptedString(twilio.AccountSid), sender.RealmId),
          manager.Decrypt(new EncryptedString(twilio.AuthenticationToken), sender.RealmId));
      default:
        throw new SenderProviderNotSupportedException(sender.Provider);
    }
  }

  public static TemplateContent Decrypt(this IEncryptionManager manager, TemplateContent content, RealmId? realmId)
  {
    string text = manager.Decrypt(new EncryptedString(content.Text), realmId);
    return new TemplateContent(content.Type, text);
  }

  public static TemplateContent Encrypt(this IEncryptionManager manager, TemplateContent content, RealmId? realmId)
  {
    string text = manager.Encrypt(content.Text, realmId).Value;
    return new TemplateContent(content.Type, text);
  }

  public static IReadOnlyDictionary<string, string> Encrypt(this IEncryptionManager manager, Variables variables, RealmId? realmId)
  {
    IReadOnlyDictionary<string, string> decrypted = variables.AsDictionary();
    Dictionary<string, string> encrypted = new(capacity: decrypted.Count);
    foreach (KeyValuePair<string, string> variable in decrypted)
    {
      encrypted[variable.Key] = manager.Encrypt(variable.Value, realmId).Value;
    }
    return encrypted.AsReadOnly();
  }
}
