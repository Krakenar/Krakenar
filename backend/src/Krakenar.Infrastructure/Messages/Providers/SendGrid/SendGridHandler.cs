using Krakenar.Core.Messages;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Templates;
using Logitar.Net.Mail;
using Logitar.Net.Mail.SendGrid;

namespace Krakenar.Infrastructure.Messages.Providers.SendGrid;

public class SendGridHandler : IMessageHandler
{
  protected virtual SendGridClient Client { get; }

  public SendGridHandler(SendGridSettings settings)
  {
    Client = new(settings.ApiKey);
  }

  public virtual void Dispose()
  {
    Client.Dispose();
    GC.SuppressFinalize(this);
  }

  public virtual async Task<SendMailResult> SendAsync(Message message, Content body, CancellationToken cancellationToken)
  {
    MailMessage mailMessage = message.ToMailMessage(body);
    SendMailResult result = await Client.SendAsync(mailMessage, cancellationToken);
    return result;
  }
}
