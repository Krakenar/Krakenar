using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;
using Message = Krakenar.Core.Messages.Message;
using Recipient = Krakenar.Core.Messages.Recipient;

namespace Krakenar.Infrastructure.Messages.Providers.SendGrid;

public static class SendGridExtensions
{
  public static MailMessage ToMailMessage(this Message message)
  {
    MailMessage mailMessage = new()
    {
      From = message.Sender.ToMailAddress(),
      Subject = message.Subject.Value,
      Body = message.Body.Text,
      IsBodyHtml = message.Body.Type == MediaTypeNames.Text.Html
    };

    foreach (Recipient recipient in message.Recipients)
    {
      MailAddress address = recipient.ToMailAddress();
      switch (recipient.Type)
      {
        case RecipientType.Bcc:
          mailMessage.Bcc.Add(address);
          break;
        case RecipientType.CC:
          mailMessage.CC.Add(address);
          break;
        case RecipientType.To:
          mailMessage.To.Add(address);
          break;
      }
    }

    return mailMessage;
  }

  public static MailAddress ToMailAddress(this SenderSummary sender)
  {
    if (sender.Email is null)
    {
      throw new ArgumentException($"The sender must be an {nameof(SenderKind.Email)} sender in order to be converted into a {nameof(MailAddress)}.", nameof(sender));
    }
    return new(sender.Email.Address, sender.DisplayName?.Value);
  }

  public static MailAddress ToMailAddress(this Recipient recipient)
  {
    if (recipient.Email is null)
    {
      throw new ArgumentException($"A recipient requires an email address in order to be converted into a {nameof(MailAddress)}.", nameof(recipient));
    }
    return new(recipient.Email.Address, recipient.DisplayName?.Value);
  }
}
