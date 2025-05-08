using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;

namespace Krakenar.Infrastructure.Messages.Providers.Twilio;

internal class TwilioStrategy : IProviderStrategy
{
  public SenderProvider Provider { get; } = SenderProvider.Twilio;

  public IMessageHandler Execute(SenderSettings settings)
  {
    return new TwilioHandler((TwilioSettings)settings);
  }
}
