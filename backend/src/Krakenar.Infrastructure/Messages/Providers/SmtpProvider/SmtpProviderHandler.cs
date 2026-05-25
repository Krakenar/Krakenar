using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Logitar.Net.Mail;
using MailKit.Security;
using MimeKit;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;
using Message = Krakenar.Core.Messages.Message;
using Recipient = Krakenar.Core.Messages.Recipient;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using SmtpProviderSettings = Krakenar.Core.Senders.Settings.SmtpProviderSettings;

namespace Krakenar.Infrastructure.Messages.Providers.SmtpProvider;

public class SmtpProviderHandler : IMessageHandler
{
  protected virtual SmtpProviderSettings Settings { get; }

  public SmtpProviderHandler(SmtpProviderSettings settings)
  {
    Settings = settings;
  }

  public void Dispose()
  {
  }

  public virtual async Task<SendMailResult> SendAsync(Message message, Content body, CancellationToken cancellationToken)
  {
    using SmtpClient client = new();
    await client.ConnectAsync(Settings.Host, Settings.Port, GetSecureSocketOptions(Settings.Security), cancellationToken);
    await client.AuthenticateAsync(Settings.Username, Settings.Password, cancellationToken);

    MimeMessage mimeMessage = ToMimeMessage(message, body);
    string response = await client.SendAsync(mimeMessage, cancellationToken);

    await client.DisconnectAsync(quit: true, cancellationToken);

    return new SendMailResult(succeeded: true, new Dictionary<string, object?>
    {
      ["Response"] = response
    });
  }

  protected virtual SecureSocketOptions GetSecureSocketOptions(SmtpSecurityMode security) => security switch
  {
    SmtpSecurityMode.Auto => SecureSocketOptions.Auto,
    SmtpSecurityMode.None => SecureSocketOptions.None,
    SmtpSecurityMode.SslOnConnect => SecureSocketOptions.SslOnConnect,
    SmtpSecurityMode.StartTls => SecureSocketOptions.StartTls,
    SmtpSecurityMode.StartTlsWhenAvailable => SecureSocketOptions.StartTlsWhenAvailable,
    _ => throw new ArgumentOutOfRangeException(nameof(security)),
  };

  protected virtual MimeMessage ToMimeMessage(Message message, Content body)
  {
    MimeMessage mimeMessage = new();
    mimeMessage.From.Add(ToMailboxAddress(message.Sender));
    mimeMessage.Subject = message.Subject.Value;

    BodyBuilder bodyBuilder = new();
    switch (body.Type)
    {
      case MediaTypeNames.Text.Plain:
        bodyBuilder.TextBody = body.Text;
        break;
      case MediaTypeNames.Text.Html:
        bodyBuilder.HtmlBody = body.Text;
        break;
    }
    mimeMessage.Body = bodyBuilder.ToMessageBody();

    foreach (Recipient recipient in message.Recipients)
    {
      MailboxAddress address = ToMailboxAddress(recipient);
      switch (recipient.Type)
      {
        case RecipientType.Bcc:
          mimeMessage.Bcc.Add(address);
          break;
        case RecipientType.CC:
          mimeMessage.Cc.Add(address);
          break;
        case RecipientType.To:
          mimeMessage.To.Add(address);
          break;
      }
    }

    return mimeMessage;
  }

  protected virtual MailboxAddress ToMailboxAddress(SenderSummary sender)
  {
    if (sender.Email is null)
    {
      throw new ArgumentException($"The sender must be an {nameof(SenderKind.Email)} sender in order to be converted into a {nameof(MailboxAddress)}.", nameof(sender));
    }
    return new(sender.DisplayName?.Value, sender.Email.Address);
  }

  protected virtual MailboxAddress ToMailboxAddress(Recipient recipient)
  {
    if (recipient.Email is null)
    {
      throw new ArgumentException($"A recipient requires an email address in order to be converted into a {nameof(MailboxAddress)}.", nameof(recipient));
    }
    return new(recipient.DisplayName?.Value, recipient.Email.Address);
  }
}
