using Krakenar.Core.Messages;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Templates;
using Logitar.Net.Mail;

namespace Krakenar.Infrastructure.Messages.Providers.SmtpProvider;

public class SmtpProviderHandler : IMessageHandler
{
  public SmtpProviderHandler(SmtpProviderSettings settings)
  {
    // TODO(fpion): implement
  }

  public void Dispose()
  {
    // TODO(fpion): implement
  }

  public virtual async Task<SendMailResult> SendAsync(Message message, Content body, CancellationToken cancellationToken)
  {
    MailMessage mailMessage = message.ToMailMessage(body);
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
