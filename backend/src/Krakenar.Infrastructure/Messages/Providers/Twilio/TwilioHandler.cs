using Krakenar.Core.Messages;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Templates;
using Logitar.Net.Mail;
using Logitar.Net.Sms;
using Logitar.Net.Sms.Twilio;

namespace Krakenar.Infrastructure.Messages.Providers.Twilio;

public class TwilioHandler : IMessageHandler
{
  protected virtual TwilioClient Client { get; }

  public TwilioHandler(TwilioSettings settings)
  {
    Client = new(settings.AccountSid, settings.AuthenticationToken);
  }

  public virtual void Dispose()
  {
    Client.Dispose();
    GC.SuppressFinalize(this);
  }

  public virtual async Task<SendMailResult> SendAsync(Message message, Content body, CancellationToken cancellationToken)
  {
    SmsMessage smsMessage = message.ToSmsMessage(body);
    SendSmsResult result = await Client.SendAsync(smsMessage, cancellationToken);
    return new SendMailResult(result.Succeeded, result.Data.ToDictionary());
  }
}
