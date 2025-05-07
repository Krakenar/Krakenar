using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Users;
using Logitar.Net.Sms;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;
using Message = Krakenar.Core.Messages.Message;
using Recipient = Krakenar.Core.Messages.Recipient;

namespace Krakenar.Infrastructure.Messages.Providers.Twilio;

public static class TwilioExtensions
{
  public static SmsMessage ToSmsMessage(this Message message)
  {
    Recipient[] recipients = message.Recipients.Where(recipient => recipient.Type == RecipientType.To).ToArray();
    if (recipients.Length != 1)
    {
      throw new ArgumentException($"Exactly one {nameof(RecipientType.To)} recipient must be provided.", nameof(message));
    }
    Recipient recipient = recipients.Single();
    if (recipient.Phone is null)
    {
      throw new ArgumentException($"The recipient requires a {nameof(Recipient.Phone)} to receive a SMS message.", nameof(message));
    }

    if (message.Sender.Phone is null)
    {
      throw new ArgumentException($"The sender must be a {nameof(SenderKind.Phone)} sender in order to send a SMS message.", nameof(message));
    }

    if (message.Body.Type != MediaTypeNames.Text.Plain)
    {
      throw new ArgumentException($"The SMS message text contents must be '{MediaTypeNames.Text.Plain}'. The content type '{message.Body.Type}' is not supported.", nameof(message));
    }

    return new SmsMessage(message.Sender.Phone.FormatToE164(), recipient.Phone.FormatToE164(), message.Body.Text);
  }
}
