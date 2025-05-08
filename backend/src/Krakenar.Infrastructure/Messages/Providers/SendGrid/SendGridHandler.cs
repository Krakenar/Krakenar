using Krakenar.Core.Messages;
using Krakenar.Core.Senders.Settings;
using Logitar.Net.Mail;
using Logitar.Net.Mail.SendGrid;

namespace Krakenar.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridHandler : IMessageHandler
{
  private readonly SendGridClient _client;

  public SendGridHandler(SendGridSettings settings)
  {
    _client = new(settings.ApiKey);
  }

  public void Dispose()
  {
    _client.Dispose();
  }

  public async Task<SendMailResult> SendAsync(Message message, CancellationToken cancellationToken)
  {
    MailMessage mailMessage = message.ToMailMessage();
    SendMailResult result = await _client.SendAsync(mailMessage, cancellationToken);
    return result;
  }
}
