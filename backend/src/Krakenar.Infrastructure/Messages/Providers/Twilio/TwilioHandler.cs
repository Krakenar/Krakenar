using Krakenar.Core.Messages;
using Krakenar.Core.Senders.Settings;
using Logitar.Net.Mail;
using Logitar.Net.Sms;
using Logitar.Net.Sms.Twilio;

namespace Krakenar.Infrastructure.Messages.Providers.Twilio;

internal class TwilioHandler : IMessageHandler
{
  private readonly TwilioClient _client;

  public TwilioHandler(TwilioSettings settings)
  {
    _client = new(settings.AccountSid, settings.AuthenticationToken);
  }

  public void Dispose()
  {
    _client.Dispose();
  }

  public async Task<SendMailResult> SendAsync(Message message, CancellationToken cancellationToken)
  {
    SmsMessage smsMessage = message.ToSmsMessage();
    SendSmsResult result = await _client.SendAsync(smsMessage, cancellationToken);
    return new SendMailResult(result.Succeeded, result.Data.ToDictionary());
  }
}
